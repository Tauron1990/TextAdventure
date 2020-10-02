using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomDescriptionRevertEvent", 1)]
    public sealed class RoomDescriptionRevertEvent : AggregateEvent<Internal.Data.Aggregates.Room, RoomId>
    {
        public bool Detail { get; }

        public RoomDescriptionRevertEvent(bool detail) => Detail = detail;
    }
}