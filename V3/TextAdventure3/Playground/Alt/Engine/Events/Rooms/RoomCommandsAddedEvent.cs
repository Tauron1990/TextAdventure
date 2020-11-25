using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCommandsAddedEvent", 1)]
    public sealed class RoomCommandsAddedEvent : AggregateEvent<Room, RoomId>
    {
        public RoomCommandsAddedEvent(CommandLayer[] layers, RoomId room)
        {
            Layers = layers;
            Room   = room;
        }

        public CommandLayer[] Layers { get; }

        public RoomId Room { get; }
    }
}