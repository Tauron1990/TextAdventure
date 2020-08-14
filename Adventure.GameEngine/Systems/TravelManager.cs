using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Events;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.ReactiveData;
using EcsRx.Systems;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [PublicAPI]
    public sealed class TravelManager : MultiEventReactionSystem, IReactToEntitySystem
    {
        private IEntity? _currentRoom;

        public TravelManager([NotNull] IEventSystem eventSystem) : base(eventSystem)
        { }

        public override IGroup Group { get; } = new Group(typeof(Room), typeof(RoomData));

        protected override void Init()
        {
            Receive<GotoDirection>(HandleMovement);
        }

        private void HandleMovement(GotoDirection move)
        {
            if(_currentRoom == null) return;

            var data = _currentRoom.GetComponent<RoomData>();

            var potenalDoor = data.Connections.Cast<IDoorway>().Concat(data.DoorWays).FirstOrDefault(dw => dw.Direction == move.Direction);
            if (potenalDoor != null)
            {
                if (string.IsNullOrWhiteSpace(potenalDoor.UnlockEvent))
                {
                    MoveToRoom(potenalDoor.TargetRoom, data);
                    move.Result = new LazyString(GameConsts.NewRoomEntered).AddParameters(StringParameter.Resolved(potenalDoor.TargetRoom));
                }
                else
                    move.Result = new LazyString(GameConsts.DoorwayLooked);
            }
            else
                move.Result = new LazyString(GameConsts.NoDoorwayFound).AddParameters(move.Original);
        }

        private void MoveToRoom(string name, RoomData from)
        {
            var to = ObservableGroup.First(e => e.GetComponent<Room>().Name == name).GetComponent<RoomData>();

            from.IsPlayerIn.Value = false;
            to.IsPlayerIn.Value = true;
            to.IsVisited.Value = true;
        }

        public IObservable<IEntity> ReactToEntity(IEntity entity) 
            => entity.GetComponent<RoomData>().IsPlayerIn.Where(b => b).Select(b => entity);

        public void Process(IEntity entity)
            => _currentRoom = entity;
    }
}