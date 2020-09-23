using TextAdventures.Builder.Internal;

namespace TextAdventures.Engine.Internal.Messages
{
    public sealed class StartGame
    {
        public WorldImpl World { get; }

        public bool NewGame { get; }

        public StartGame(WorldImpl world, bool newGame)
        {
            World = world;
            NewGame = newGame;
        }
    }
}