using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public static class Game
    {
        public static GameMaster Create(string gameName, World world)
        {
            var system = ActorSystem.Create(gameName);
            var gameMaster = system.ActorOf(Props.Create(() => new GameMasterActor()), "GameMaster");
            gameMaster.Tell(world.CreateSetup());
            
            return new GameMaster(gameMaster, system);
        }
    }
}