using System;
using Akka.Actor;
using TextAdventures.Builder;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.EventSystem;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine.Actors
{
    internal sealed class GameMasterActor : GameProcess
    {
        private bool              _isRunning;
        private Action<Exception> _errorHandler = _ => {};
        
        private readonly IActorRef _gameObjectManager;
        private readonly IActorRef _eventDispatcher;

        public GameMasterActor(GameProfile profile)
        {
            _eventDispatcher = Context.ActorOf(Props.Create(() => new EventDispatcherActor()));
            _gameObjectManager = Context.ActorOf(Props.Create(() => new GameObjectManagerActor()));
            var saveGameManager = Context.ActorOf(Props.Create(() => new SaveGameManagerActor(profile, _gameObjectManager)));

            Context.System.RegisterExtension(new GameCore.GameCoreId(
                                                                     new EventDispatcher(_eventDispatcher), 
                                                                     new GameObjectManager(_gameObjectManager),
                                                                     new GameMaster(Self, Context.System)));

            Receive<GameEvent>(evt => _eventDispatcher.Forward(evt));
            Receive<RequestEventSource>(r => _eventDispatcher.Forward(r));

            Receive<GameObjectBlueprint>(p => _gameObjectManager.Forward(p));
            Receive<IGameCommand>(c => _gameObjectManager.Forward(c));
            Receive<RegisterCommandProcessorBase>(r => _gameObjectManager.Forward(r));

            Receive<MakeSaveGame>(s => saveGameManager.Forward(s));

            Receive<string>(CompledLoading);
            Receive<GameSetup>(SetupGame);
        }

        private void CompledLoading(string obj)
        {
            if (_isRunning) return;

            _isRunning = true;

            foreach (var child in Context.GetChildren())
                child.Tell(new LoadingCompled());

            _eventDispatcher.Tell(new LoadingCompled());
        }

        private void SetupGame(GameSetup obj)
        {
            if(_isRunning) return;

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