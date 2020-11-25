using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomDescriptionRevertEvent", 1)]
    public sealed class RoomDescriptionRevertEvent : AggregateEvent<Room, RoomId>
    {
        public RoomDescriptionRevertEvent(bool detail) => Detail = detail;
        public bool Detail { get; }
    }
}