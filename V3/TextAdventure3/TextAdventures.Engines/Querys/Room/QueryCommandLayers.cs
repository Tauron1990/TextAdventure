using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Querys.Room
{
    public sealed class QueryCommandLayers : RoomQueryBase
    {
        public RoomId Id { get; }

        public QueryCommandLayers(RoomId id) => Id = id;
    }
}