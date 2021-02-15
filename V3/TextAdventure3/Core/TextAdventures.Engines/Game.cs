using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Modules;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine
{
    public sealed record GameConfiguration(string GameName, string ProfileName, string SaveName,
        bool AddDefaultModules = true);

    [PublicAPI]
    public static class Game
    {
        public static GameMaster Create(GameConfiguration configuration, World world)
        {
            var (gameName, profileName, saveName, addDefaultModules) = configuration;

            if (addDefaultModules)
                DefaultModules.AddDefaultModules(world);

            foreach (var worldModule in world.Modules)
                worldModule.Enrich(world);

            var system = ActorSystem.Create(gameName);

            var gameMaster =
                system.ActorOf(Props.Create(() => new GameMasterActor(GameProfile.GetDefault(gameName, profileName))),
                    "GameMaster");
            gameMaster.Tell(world.CreateSetup() with {SaveGame = saveName});

            return new GameMaster(gameMaster, system);
        }
    }
}