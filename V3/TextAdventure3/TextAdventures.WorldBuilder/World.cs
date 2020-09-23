using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Internal;

namespace TextAdventures.Builder
{
    public abstract class World
    {
        internal World()
        {
            
        }

        public static World Create(string saveFile)
            => new WorldImpl(saveFile);

        public abstract RoomBuilder GetRoom(Name name);
    }
}