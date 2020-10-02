using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Builder.Internal;

namespace TextAdventures.Builder.Builder
{
    public sealed class RoomBuilder
    {
        private readonly WorldImpl _worldImpl;
        private ImmutableDictionary<Direction, Doorway> _doorways = ImmutableDictionary<Direction, Doorway>.Empty;
        private ImmutableList<CommandLayer> _commandLayers = ImmutableList<CommandLayer>.Empty;

        public RoomId Self { get; }
        public Name Name { get; }

        public IReadOnlyDictionary<Direction, Doorway> Doorways => _doorways;

        public IReadOnlyList<CommandLayer> CommandLayers => _commandLayers;

        public Description Description { get; private set; } = new Description(string.Empty);

        public Description DestailDescription { get; private set; } = new Description(string.Empty);

        public RoomBuilder(WorldImpl worldImpl, RoomId self, Name name)
        {
            _worldImpl = worldImpl;
            Self = self;
            Name = name;
        }

        public RoomBuilder WithDetailDescription(Description description)
        {
            DestailDescription = description;
            return this;
        }

        public RoomBuilder WithDescription(Description description)
        {
            Description = description;
            return this;
        }

        public RoomBuilder WithDoorway(params DoorwayBuilder[] doorways)
        {
            foreach (var doorway in doorways.Select(b => new Doorway(Self, b.Target, b.LockState, b.Direction)))
            {
                _doorways = _doorways.Add(doorway.Direction, doorway);
                _worldImpl.GetRoom(doorway.Target).AddOpposit(doorway);

            }

            return this;
        }

        public RoomBuilder WithCommandlayer(params CommandLayer[] layers)
        {
            _commandLayers = _commandLayers.AddRange(layers);

            return this;
        }

        private void AddOpposit(Doorway way)
        {
            Direction direction;

            if(Direction.East == way.Direction)
                direction = Direction.West;
            else if(Direction.North == way.Direction)
                direction = Direction.South;
            else if(Direction.West == way.Direction)
                direction = Direction.East;
            else if(Direction.South == way.Direction)
                direction = Direction.North;

            else if(Direction.NorthEast == way.Direction)
                direction = Direction.NorthWest;
            else if(Direction.NorthWest == way.Direction)
                direction = Direction.NorthEast;
            else if(Direction.SouthEast == way.Direction)
                direction = Direction.SouthWest;
            else if (Direction.SouthWest == way.Direction)
                direction = Direction.SouthEast;

            else
                direction = way.Direction;

            _doorways = _doorways.Add(direction, new Doorway(Self, _worldImpl.FindById(way.Id).Name, way.Locked, direction));
        }
    }
}