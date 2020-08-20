using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Blueprints
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
        }
    }
}