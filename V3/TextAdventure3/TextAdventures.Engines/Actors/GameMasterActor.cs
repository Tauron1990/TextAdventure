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
            IActorRef eventDispatcher = Context.ActorOf(Props.Create(() => new EventDispatcherActor()));
            _gameObjectManager = Context.ActorOf(Props.Create(() => new GameObjectManagerActor()));

            Context.System.RegisterExtension(new GameCore.GameCoreId(new EventDispatcher(eventDispatcher), new GameObjectManager(_gameObjectManager)));

            Receive<GameEvent>(evt => eventDispatcher.Forward(evt));
            Receive<RequestEventSource>(r => eventDispatcher.Forward(r));
            Receive<GameObjectBlueprint>(p => _gameObjectManager.Forward(p));

            Receive<string>(CompledLoading);
            Receive<GameSetup>(SetupGame);
        }

        private void CompledLoading(string obj)
        {
            foreach (var child in Context.GetChildren())
                child.Tell(new LoadingCompled());
        }

        private void SetupGame(GameSetup obj)
        {
            if(_isRunning) return;

            _isRunning = true;

            _errorHandler = obj.Error;

            foreach (var (name, process) in obj.GameProcesses) 
                Context.ActorOf(process, name);

            Context.ActorOf(Props.Create(() => new LoadCoordinator(_gameObjectManager, Self))).Tell(obj);
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
            private readonly IActorRef _master;

            public LoadCoordinator(IActorRef objectManager, IActorRef master)
            {
                _objectManager = objectManager;
                _master = master;

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
                         if (evt.FsmEvent is LoadingCompled)
                         {
                             Context.Stop(Self);
                             return Stay();
                         }

                         _master.Tell("new LoadingCompled()", ActorRefs.NoSender);
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
    }

    public sealed record LoadingCompled;
}