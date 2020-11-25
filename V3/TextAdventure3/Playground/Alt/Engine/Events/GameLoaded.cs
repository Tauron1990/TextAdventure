using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Events
{
    public sealed class GameLoaded : TransistentEvent<GameLoaded>
    {
        public GameLoaded(SaveProfile info, GameMaster master)
        {
            Info   = info;
            Master = master;
        }

        public SaveProfile Info { get; }

        public GameMaster Master { get; }
    }
}