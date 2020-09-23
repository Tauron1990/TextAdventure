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
    }
}