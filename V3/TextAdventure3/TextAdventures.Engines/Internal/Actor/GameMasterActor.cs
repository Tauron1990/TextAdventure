using Tauron.Akka;
using TextAdventures.Engine.Internal.Messages;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class GameMasterActor : ExposedReceiveActor
    {
        public GameMasterActor()
        {
            Receive<StartGame>(InitializeGame);
        }

        private void InitializeGame(StartGame obj)
        {
            //if(obj.NewGame)

        }
    }
}