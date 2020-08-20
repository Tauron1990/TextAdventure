using Adventure.GameEngine;
using Adventure.GameEngine.Builder;
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
            Content.AddContentItem(GameConsts.NoDoorwayFound, "Kein weg in der Richtung \"{0}\" gefunden.");
            Content.AddContentItem(GameConsts.DoorwayLooked, "De weg ist verspeert.");
            Content.AddContentItem(GameConsts.NewRoomEntered, "Du bist jetch in \"{0}\"");
            Content.AddContentItem("Start", "Einwohner melde Amt");
            Content.AddContentItem(GameConsts.NoObjectForPickupFound, "Das Objekt \"{0}\" ist nicht hier!");
            Content.AddContentItem(GameConsts.ObjectUnaleToPickup, "Das Objekt\"{0}\" kann ich nicht ehmen.");
            Content.AddContentItem(GameConsts.ObjectPickedUp, "Das object {0} ist jetzt im Inventar.");
        }

        protected override RoomBuilder ConfigurateRooms(RoomConfiguration configuration)
        {
            return configuration.NewRoom("Start")
                .WithCommonCommandSet()
                .WithDescription("Einstigpunkt des Spiels");
        }
    }
}