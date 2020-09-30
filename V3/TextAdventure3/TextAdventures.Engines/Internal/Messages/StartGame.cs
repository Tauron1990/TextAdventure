using TextAdventures.Builder.Internal;
using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Internal.Messages
{
    public sealed class StartGame
    {
        public WorldImpl World { get; }

        public bool NewGame { get; }

        public SaveProfile SaveGame { get; }

        public SaveInfo? SaveInfo { get; }

        public StartGame(WorldImpl world, bool newGame, SaveProfile saveGame, SaveInfo? saveInfo)
        {
            World = world;
            NewGame = newGame;
            SaveGame = saveGame;
            SaveInfo = saveInfo;
        }
    }
}