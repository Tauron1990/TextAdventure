using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Events
{
    [EventVersion("RoomCreated", 1)]
    public sealed class RoomCreatedEvent : AggregateEvent<Room, RoomId>
    {
        public Name Name { get; set; }

        public RoomCreatedEvent(Name name) 
            => Name = name;
    }
}