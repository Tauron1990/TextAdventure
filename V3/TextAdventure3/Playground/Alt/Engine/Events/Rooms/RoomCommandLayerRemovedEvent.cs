using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Rooms
{
    [EventVersion("RoomCommandLayerRemovedEvent", 1)]
    public sealed class RoomCommandLayerRemovedEvent : AggregateEvent<Room, RoomId>
    {
        public RoomCommandLayerRemovedEvent(RoomId roomId, Name name)
        {
            RoomId = roomId;
            Name   = name;
        }

        public RoomId RoomId { get; }

        public Name Name { get; }
    }
}