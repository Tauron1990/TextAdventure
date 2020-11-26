using System;
using Akka.Actor;
using TextAdventures.Builder;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.EventSystem;

namespace TextAdventures.Engine.Actors
{
    internal sealed class GameMasterActor : GameProcess
    {
        private bool              _isRunning;
        private Action<Exception> _errorHandler = _ => {};
        
        private readonly IActorRef _gameObjectManager;
        
        public GameMasterActor()
        {
            IActorRef eventDispatcher = Context.ActorOf(Props.Create(() => new EventDispatcher()));
            _gameObjectManager = Context.ActorOf(Props.Create(() => new GameObjectManager()));
            
            Receive<GameEvent>(evt => eventDispatcher.Forward(evt));
            Receive<RequestEventSource>(r => eventDispatcher.Forward(r));
            Receive<GameObjectBlueprint>(p => _gameObjectManager.Forward(p));

            Receive<LoadingCompled>(CompledLoading);
            Receive<GameSetup>(SetupGame);
        }

        private void CompledLoading(LoadingCompled obj)
        {
            
        }

        private void SetupGame(GameSetup obj)
        {
            if(_isRunning) return;

            _isRunning = true;

            foreach (var (name, process) in obj.GameProcesses) 
                Context.ActorOf(process, name);

            Context.ActorOf(Props.Create(() => new LoadCoordinator(_gameObjectManager))).Tell(obj);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            var system = Context.System;
            return new OneForOneStrategy(e =>
                                         {
                                             _errorHandler(e);
                                             system.Terminate();
                                             
                                             return Directive.Stop;
                                         });
        }
        
        private sealed class LoadCoordinator : FSM<LoadCoordinator.LoadStep, LoadCoordinator.LoadCoordinatorState>
        {
            private readonly IActorRef         _objectManager;

            public LoadCoordinator(IActorRef objectManager)
            {
                _objectManager = objectManager;

                InitFsm();
            }

            private void InitFsm()
            {
                When(LoadStep.InitObjectManager,
                     evt =>
                     {
                         if (evt.FsmEvent is not GameSetup gameSetup) return Stay();
                         
                         _objectManager.Tell(gameSetup);
                         return GoTo(LoadStep.StartingFinish);
                     });
                
                When(LoadStep.StartingFinish,
                     evt =>
                     {
                         Context.Parent.Tell(new LoadingCompled());
                         Context.Stop(Self);
                         return Stay();
                     });
                
                StartWith(LoadStep.InitObjectManager, new LoadCoordinatorState());
                Initialize();
            }
            
            public sealed record LoadCoordinatorState;
            
            public enum LoadStep
            {
                InitObjectManager,
                StartingFinish
            }
        }

        private sealed record LoadingCompled;
    }
}