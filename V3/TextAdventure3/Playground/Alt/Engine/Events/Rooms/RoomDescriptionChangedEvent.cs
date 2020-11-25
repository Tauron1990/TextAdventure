using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomDescriptionChangedEvent", 1)]
    public sealed class RoomDescriptionChangedEvent : AggregateEvent<Room, RoomId>
    {
        public RoomDescriptionChangedEvent(Description description, bool isDetail)
        {
            Description = description;
            IsDetail    = isDetail;
        }

        public Description Description { get; }

        public bool IsDetail { get; }
    }
}