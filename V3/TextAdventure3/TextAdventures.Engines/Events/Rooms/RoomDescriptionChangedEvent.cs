using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomDescriptionChangedEvent", 1)]
    public sealed class RoomDescriptionChangedEvent : AggregateEvent<Internal.Data.Aggregates.Room, RoomId>
    {
        public Description Description { get; }

        public bool IsDetail { get; }

        public RoomDescriptionChangedEvent(Description description, bool isDetail)
        {
            Description = description;
            IsDetail = isDetail;
        }
    }
}