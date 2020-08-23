using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Adventure.GameEngine.Systems.Components;
using Adventure.GameEngine.Systems.Events;
using BattlePlanPath;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Custom;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class Katographer : EventReactionSystem<RequestPath>
    {
        private sealed class Graph : IPathGraph<string, IObservableGroup>
        {
            private readonly Dictionary<string, HashSet<string>> _grid;

            public Graph(Dictionary<string, HashSet<string>> grid) => _grid = grid;

            public IEnumerable<string> Neighbors(string fromNode) 
                => _grid.TryGetValue(fromNode, out var list) ? list : Enumerable.Empty<string>();

            public double Cost(string fromNode, string toNode, IObservableGroup callerData) => 1;

            public double EstimatedCost(string fromNode, string toNode, IObservableGroup callerData) => 1;
        }

        private IDisposable _mapBuildRegistration = Disposable.Empty;
        private readonly Dictionary<string, HashSet<string>> _connections = new Dictionary<string, HashSet<string>>();

        public Katographer(IEventSystem eventSystem) 
            : base(eventSystem) { }

        public override IGroup Group { get; } = new Group(typeof(RoomData), typeof(Room));

        public override void StartSystem(IObservableGroup observableGroup)
        {
            _mapBuildRegistration = EventSystem.Receive<GameBuild>().Subscribe(MapBuild);
            base.StartSystem(observableGroup);
        }

        public override void StopSystem(IObservableGroup observableGroup)
        {
            _mapBuildRegistration.Dispose();
            base.StopSystem(observableGroup);
        }

        private void MapBuild(GameBuild build)
        {
            foreach (var entity in ObservableGroup)
            {
                _connections[entity.GetComponent<Room>().Name] 
                    = new HashSet<string>(entity.GetComponent<RoomData>()
                        .DoorWays.Select(d => d.TargetRoom)
                        .Concat(entity.GetComponent<RoomData>().Connections.Select(c => c.TargetRoom)));
            }
        }

        public override void EventTriggered(RequestPath eventData)
        {
            try
            {
                var pathSolver = new PathSolver<string, IObservableGroup>(new Graph(_connections));

                pathSolver.BuildAdjacencyGraph(new []{ eventData.From, eventData.To });

                eventData.Path = pathSolver.FindPath(eventData.From, eventData.To, ObservableGroup);
            }
            catch
            {
                eventData.Path = null;
            }

        }
    }
}