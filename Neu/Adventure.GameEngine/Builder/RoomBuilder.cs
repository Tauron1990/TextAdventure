using System;
using System.Collections.Generic;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class RoomBuilder
    {
        private readonly HashSet<Direction> _locked = new HashSet<Direction>();
        private readonly Func<string, RoomBuilder?> _roomLookup;

        internal RoomBuilder(string name, RoomConfiguration root, Func<string, RoomBuilder?> roomLookup)
        {
            Blueprints.Add(new PersitBlueprint(name));
            Blueprints.Add(new RoomCore(name));
            Name = name;
            Root = root;
            _roomLookup = roomLookup;
        }

        internal string Name { get; }

        public RoomConfiguration Root { get; }

        internal List<IBlueprint> Blueprints { get; } = new List<IBlueprint>();

        internal List<IBluePrintProvider> NewEntities { get; } = new List<IBluePrintProvider>();

        internal List<DoorWay> DoorWays { get; } = new List<DoorWay>();

        internal List<DoorWayConnection> Connections { get; } = new List<DoorWayConnection>();

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
            => Root;

        internal bool Connect(string from, DoorWay original)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            var realDirection = original.Direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                Direction.NorthEast => Direction.SouthWest,
                Direction.SouthEast => Direction.NorthWest,
                Direction.SouthWest => Direction.NorthEast,
                Direction.NorthWest => Direction.SouthEast,
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                _ => Direction.Custom
            };

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