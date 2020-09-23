using System.Collections.Generic;
using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Data;

namespace TextAdventures.Builder.Internal
{
    public sealed class WorldImpl : World
    {
        public string SaveGame { get; }

        public Dictionary<Name, RoomBuilder> Rooms { get; } = new Dictionary<Name, RoomBuilder>();

        internal WorldImpl(string saveGame) => SaveGame = saveGame;
        public override RoomBuilder GetRoom(Name name)
        {
            if (Rooms.TryGetValue(name, out var room))
                return room;
            
            room = new RoomBuilder(this);
            Rooms.Add(name, room);
            return room;
        }
    }
}