using Tauron.Akka;
using TextAdventures.Engines.Internal.Messages;

namespace TextAdventures.Engines.Internal.Actor
{
    public sealed class GameMasterActor : ExposedReceiveActor
    {
        public GameMasterActor()
        {
            Receive<StartGame>(InitializeGame);
        }

        private void InitializeGame(StartGame obj)
        {
            if(obj.NewGame)

        }
    }
}