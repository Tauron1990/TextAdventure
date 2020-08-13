using Adventure.GameEngine;
using Adventure.GameEngine.Rooms;
using Adventure.Utilities;

namespace TextAdventure
{
    public sealed class AdventureGame : Game
    {
        public AdventureGame(string saveGame, IStartUpNotify notify, ContentManagement content) 
            : base(saveGame, notify, content)
        {
        }

        protected override int Version { get; } = 1;

        protected override RoomBuilder ConfigurateRooms(RoomConfiguration configuration)
        {
            return configuration.NewRoom("Start");
        }
    }
}