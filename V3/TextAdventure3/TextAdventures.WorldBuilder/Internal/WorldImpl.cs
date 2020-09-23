namespace TextAdventures.Builder.Internal
{
    public sealed class WorldImpl : World
    {
        public string SaveGame { get; }

        internal WorldImpl(string saveGame) => SaveGame = saveGame;
    }
}