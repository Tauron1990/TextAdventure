using System;
using System.Collections.Generic;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class RoomBuilder : SubEntityConfiguration, IEventable<RoomBuilder, object>, IHasRoot
    {
        private readonly HashSet<Direction> _locked = new HashSet<Direction>();
        private readonly Func<string, RoomBuilder?> _roomLookup;

        public string Name { get; }

        public RoomBuilder(string name, IWithMetadata metadata, Func<string, RoomBuilder?> roomLookup, IContentManagement contentManagement, RoomConfiguration roomConfiguration)
            : base(metadata.Metadata)
        {
            Name = name;
            ContentManagement = contentManagement;
            RoomConfiguration = roomConfiguration;
            _roomLookup = roomLookup;

            AddBlueprint(new PersitBlueprint(name));
            AddBlueprint(new RoomCore(name));
            AddBlueprint(new RoomCommand(new LookCommand(null), LazyString.New(GameConsts.LookCommand)));
        }
        
        internal IContentManagement ContentManagement { get; }

        public RoomConfiguration RoomConfiguration { get; }
        
        internal List<DoorWay> DoorWays { get; } = new List<DoorWay>();

        internal List<DoorWayConnection> Connections { get; } = new List<DoorWayConnection>();


        public RoomBuilder WithDoorWay(DoorWay doorWay)
        {
            if (_locked.Add(doorWay.Direction))
                DoorWays.Add(doorWay);
            else
                throw new InvalidOperationException("Duplicate Doorway Direction");

            return this;
        }

        public RoomBuilder ReactOnEvent<TData>(string name, TData data)
        {
            AddBlueprint(new EntityEvent<TData>(data, name));
            return this;
        }

        public RoomConfiguration And()
            => RoomConfiguration;

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

        protected override void ValidateImpl()
        {
            foreach (var doorWay in DoorWays)
            {
                var oposit = _roomLookup(doorWay.TargetRoom);
                if (oposit == null)
                    throw new InvalidOperationException($"Room not Found {doorWay.TargetRoom}");
                if (!oposit.Connect(Name, doorWay))
                    throw new InvalidOperationException($"Connection with {doorWay.TargetRoom} Failed");
            }

            base.ValidateImpl();
        }

        RoomBuilder IEventable<RoomBuilder, object>.EventSource => this;
        object IEventable<RoomBuilder, object>.EventData => new object();
        IEntityConfiguration IHasRoot.Root => this;
    }
}