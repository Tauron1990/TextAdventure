using Adventure.GameEngine;
using Adventure.GameEngine.Rooms;
using Adventure.Utilities.Interfaces;

namespace TextAdventure
{
    public sealed class AdventureGame : Game
    {
        public AdventureGame(string saveGame, IStartUpNotify notify, IContentManagement content) 
            : base(saveGame, notify, content)
        {
        }

        protected override int Version { get; } = 1;

        protected override void LoadResiources()
        {
        }

        protected override RoomBuilder ConfigurateRooms(RoomConfiguration configuration)
        {
            return configuration.NewRoom("Start")
               .WithCommonCommandSet()
               .WithDescription("Einstigpunkt des Spiels");
        }
    }
}