using System;
using System.Collections.Generic;
using Adventure.GameEngine.Blueprints;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    [PublicAPI]
    public sealed class RoomBuilder
    {
        internal string Name { get; }
        private readonly RoomConfiguration _root;
        private readonly Func<string, RoomBuilder?> _roomLookup;
        private readonly HashSet<Direction> _locked = new HashSet<Direction>();

        internal List<IBlueprint> Blueprints { get; } = new List<IBlueprint>();
        internal List<DoorWay> DoorWays { get; } = new List<DoorWay>();

        internal List<DoorWayConnection> Connections { get; } = new List<DoorWayConnection>();

        internal RoomBuilder(string name, RoomConfiguration root, Func<string, RoomBuilder?> roomLookup)
        {
            Blueprints.Add(new RoomCore(name));
            Name = name;
            _root = root;
            _roomLookup = roomLookup;
        }

        public RoomBuilder WithBluePrint(IBlueprint print)
        {
            Blueprints.Add(print);
            return this;
        }

        public RoomBuilder WithDoorWay(DoorWay doorWay)
        {
            if (_locked.Add(doorWay.Direction))
                DoorWays.Add(doorWay);
            else
                throw new InvalidOperationException("Duplicate Doorway Direction");

            return this;
        }

        public RoomConfiguration And()
            => _root;

        internal bool Connect(string from, DoorWay original)
        {
            var realDirection = Direction.Custom;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (original.Direction)
            {
                case Direction.North:
                    realDirection = Direction.South;
                    break;
                case Direction.East:
                    realDirection = Direction.West;
                    break;
                case Direction.South:
                    realDirection = Direction.North;
                    break;
                case Direction.West:
                    realDirection = Direction.East;
                    break;
                case Direction.NorthEast:
                    realDirection = Direction.SouthWest;
                    break;
                case Direction.SouthEast:
                    realDirection = Direction.NorthWest;
                    break;
                case Direction.SouthWest:
                    realDirection = Direction.NorthEast;
                    break;
                case Direction.NorthWest:
                    realDirection = Direction.SouthEast;
                    break;
                case Direction.Up:
                    realDirection = Direction.Down;
                    break;
                case Direction.Down:
                    realDirection = Direction.Up;
                    break;
            }

            if (realDirection == Direction.Custom)
                return false;

            if (!_locked.Add(realDirection))
                return false;

            Connections.Add(new DoorWayConnection(original, realDirection, from));

            return true;
        }

        internal string? Validate()
        {
            foreach (var doorWay in DoorWays)
            {
                var oposit = _roomLookup(doorWay.TargetRoom);
                if (oposit == null)
                    return $"Room not Found {doorWay.TargetRoom}";
                if (!oposit.Connect(Name, doorWay))
                    return $"Connection with {doorWay.TargetRoom} Failed";
            }

            return null;
        }
    }
}