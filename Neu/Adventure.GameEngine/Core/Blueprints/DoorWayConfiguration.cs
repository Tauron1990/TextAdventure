using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class DoorWayConfiguration : IBlueprint
    {
        private readonly IEnumerable<DoorWay> _doorWays;
        private readonly IEnumerable<DoorWayConnection> _connections;

        public DoorWayConfiguration(IEnumerable<DoorWay> doorWays, IEnumerable<DoorWayConnection> connections)
        {
            _doorWays = doorWays;
            _connections = connections;
        }

        public void Apply(IEntity entity)
        {
            var data = entity.GetComponent<RoomData>();

            data.DoorWays = _doorWays.ToArray();
            data.Connections = _connections.ToArray();


            var commands = entity.HasComponent<RoomCommands>() ? entity.GetComponent<RoomCommands>() : entity.AddComponent(new RoomCommands());

            foreach (var doorway in _doorWays.OfType<IDoorway>().Concat(_connections).Where(dw => dw.Direction != Direction.Custom))
            {
                commands.Add(
                    LazyString.New("{0}: {0}").AddParameters(StringParameter.Resolved(doorway.Direction.ToString()), StringParameter.Resolved(doorway.TargetRoom)),
                    new DirectionTravelCommand(doorway.Direction){ Category = GameConsts.TravelCategory });
            }
        }
    }
}