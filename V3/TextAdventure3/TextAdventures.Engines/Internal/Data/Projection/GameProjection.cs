using System.Collections.Concurrent;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Internal.Data.Projection
{
    public sealed class GameProjection
    {
        public ConcurrentDictionary<RoomId, RoomProjection> Rooms { get; } = new ConcurrentDictionary<RoomId, RoomProjection>();

        public void Clear()
        {
            Rooms.Clear();
        }
    }
}