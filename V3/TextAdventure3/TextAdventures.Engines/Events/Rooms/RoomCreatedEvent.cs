using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCreated", 1)]
    public sealed class RoomCreatedEvent : AggregateEvent<Internal.Data.Aggregates.Room, RoomId>
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