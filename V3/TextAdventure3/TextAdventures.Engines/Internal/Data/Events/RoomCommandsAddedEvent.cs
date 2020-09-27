using Akkatecture.Aggregates;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Events
{
    public sealed class RoomCommandsAddedEvent : AggregateEvent<Room, RoomId>
    {
        public CommandLayer[] Layers { get; }

        public RoomId Room { get; }

        public RoomCommandsAddedEvent(CommandLayer[] layers, RoomId room)
        {
            Layers = layers;
            Room = room;
        }
    }
}