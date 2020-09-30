using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Events
{
    public sealed class GameLoaded : TransistentEvent<GameLoaded>
    {
        public SaveProfile Info { get; }

        public GameMaster Master { get; }

        public GameLoaded(SaveProfile info, GameMaster master)
        {
            Info = info;
            Master = master;
        }
    }
}