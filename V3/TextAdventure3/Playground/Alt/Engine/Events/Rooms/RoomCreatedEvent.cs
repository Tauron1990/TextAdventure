using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCreated", 1)]
    public sealed class RoomCreatedEvent : AggregateEvent<Room, RoomId>
    {
        public RoomCreatedEvent(Name name, RoomId id, Doorway[] doorways)
        {
            Name     = name;
            Id       = id;
            Doorways = doorways;
        }

        public Name Name { get; }

        public RoomId Id { get; }

        public Doorway[] Doorways { get; }
    }
}