using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCommandLayerRemovedEvent", 1)]
    public sealed class RoomCommandLayerRemovedEvent : AggregateEvent<Internal.Data.Aggregates.Room, RoomId>
    {
        public RoomId RoomId { get; }

        public Name Name { get; }

        public RoomCommandLayerRemovedEvent(RoomId roomId, Name name)
        {
            RoomId = roomId;
            Name = name;
        }
    }
}