using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Events
{
    [EventVersion("RoomCreated", 1)]
    public sealed class RoomCreatedEvent : AggregateEvent<Room, RoomId>
    {
        public Name Name { get; }

        public RoomId Id { get; }

        public Doorway[] Doorways { get; }

        public RoomCreatedEvent(Name name, RoomId id, Doorway[] doorways)
        {
            Name = name;
            Id = id;
            Doorways = doorways;
        }
    }
}