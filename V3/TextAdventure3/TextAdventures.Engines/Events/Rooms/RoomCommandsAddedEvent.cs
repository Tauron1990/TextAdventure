using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCommandsAddedEvent", 1)]
    public sealed class RoomCommandsAddedEvent : AggregateEvent<Internal.Data.Aggregates.Room, RoomId>
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