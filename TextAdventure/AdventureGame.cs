using Adventure.GameEngine;
using Adventure.Utilities;

namespace TextAdventure
{
    public sealed class AdventureGame : Game
    {
        public AdventureGame(string saveGame, IStartUpNotify notify, ContentManagement content) : base(saveGame, notify, content)
        {
        }

        protected override int Version { get; }
    }
}