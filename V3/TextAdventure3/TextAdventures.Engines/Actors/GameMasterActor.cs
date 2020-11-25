using Akka.Actor;
using TextAdventures.Builder;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.EventSystem;

namespace TextAdventures.Engine.Actors
{
    internal sealed class GameMasterActor : GameProcess
    {
        private readonly IActorRef _gameObjectManager;
        
        public GameMasterActor()
        {
            IActorRef eventDispatcher = Context.ActorOf(Props.Create(() => new EventDispatcher()));
            _gameObjectManager = Context.ActorOf(Props.Create(() => new GameObjectManager()));
            
            Receive<GameEvent>(evt => eventDispatcher.Forward(evt));
            Receive<RequestEventSource>(r => eventDispatcher.Forward(r));
            Receive<GameObjectBlueprint>(p => _gameObjectManager.Forward(p));
            
            Receive<GameSetup>(SetupGame);
        }

        private void SetupGame(GameSetup obj)
        {
            
        }
    }
}