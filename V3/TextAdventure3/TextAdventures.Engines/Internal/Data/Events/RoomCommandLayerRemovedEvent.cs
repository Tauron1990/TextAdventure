using Akkatecture.Aggregates;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Events
{
    public sealed class RoomCommandLayerRemovedEvent : AggregateEvent<Room, RoomId>
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