using Adventure.GameEngine;
using Adventure.GameEngine.BuilderAlt;
using Adventure.GameEngine.BuilderAlt.RoomData;

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
            Content.AddContentItem(GameConsts.TravelCategory, "Gehe Nach");
            Content.AddContentItem("North", "Norden");
            Content.AddContentItem("East", "Osten");
            Content.AddContentItem("South", "Süden");
            Content.AddContentItem("West", "Westen");
            Content.AddContentItem("NorthEast", "Nord Osten");
            Content.AddContentItem("SouthEast", "Süd Osten");
            Content.AddContentItem("SouthWest", "Süd Westen");
            Content.AddContentItem("NorthWest", "Nord Westen");
            Content.AddContentItem("Up", "Oben"); 
            Content.AddContentItem("Down", "Unten");

            Content.AddContentItem(GameConsts.DoorwayLooked, "De weg ist verspeert.");
            Content.AddContentItem(GameConsts.NewRoomEntered, "Du bist Jetzt in \"{0}\"");
            
            Content.AddContentItem(GameConsts.NoObjectForPickupFound, "Das Objekt \"{0}\" ist nicht hier!");
            Content.AddContentItem(GameConsts.ObjectUnaleToPickup, "Das Objekt\"{0}\" kann ich nicht ehmen.");
            Content.AddContentItem(GameConsts.ObjectPickedUp, "Das object {0} ist jetzt im Inventar.");

            Content.AddContentItem(GameConsts.LookCategory, "Umsehen");
            Content.AddContentItem(GameConsts.LookCommand, "Raum Ansehen");
            Content.AddContentItem(GameConsts.LookAtCommand, "Inspiziere {0}");
        }

        protected override RoomBuilder ConfigurateGame(GameConfiguration configuration)
        {
            var outside = configuration.Rooms.NewRoom("Außen");
            


            return configuration.AddProlog("Programm zum Testen der Spiel Engine", b => b.WithStart(outside, "Start"));
        }
    }
}