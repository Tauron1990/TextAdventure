using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Querys.Room
{
    public sealed class QueryCommandLayers : RoomQueryBase
    {
        public QueryCommandLayers(RoomId id) => Id = id;
        public RoomId Id { get; }
    }
}