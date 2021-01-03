using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IActorRef _eventDispatcher;
        private readonly IActorRef _gameObjectManager;
        private readonly IActorRef _saveGameManager;

        private Action<Exception> _errorHandler = _ => { };
        private bool _isRunning;

        public GameMasterActor(GameProfile profile)
        {
            _eventDispatcher = Context.ActorOf(Props.Create(() => new EventDispatcherActor()));
            _gameObjectManager = Context.ActorOf(Props.Create(() => new GameObjectManagerActor()));
            _saveGameManager = Context.ActorOf(Props.Create(() => new SaveGameManagerActor(profile, _gameObjectManager)));

            Context.System.RegisterExtension(new GameCore.GameCoreId(
                                                                     new EventDispatcher(_eventDispatcher),
                                                                     new GameObjectManager(_gameObjectManager),
                                                                     new GameMaster(Self, Context.System)));

            Receive<GameEvent>(evt => _eventDispatcher.Forward(evt));
            Receive<RequestEventSource>(r => _eventDispatcher.Forward(r));

            Receive<ComponentBlueprint>(b => _gameObjectManager.Tell(b));
            Receive<GameObjectBlueprint>(p => _gameObjectManager.Forward(p));
            Receive<IGameCommand>(c => _gameObjectManager.Forward(c));
            Receive<RegisterCommandProcessorBase>(r => _gameObjectManager.Forward(r));

            Receive<MakeSaveGame>(s => _saveGameManager.Forward(s));

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

        private void SetupGame(GameSetup setup)
        {
            if (_isRunning) return;

            _errorHandler = setup.Error;

            var actorRefs = setup.GameProcesses.Select(p => Context.ActorOf(p.Value, p.Key)).ToArray();

            Context.ActorOf(Props.Create(() => new LoadCoordinator(_saveGameManager, _gameObjectManager, Self, actorRefs))).Tell(setup);
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
            public enum LoadStep
            {
                PreInit,
                InitObjectManager,
                AppySaveGame,
                SetupMessages,
                StartingFinish
            }

            private readonly IActorRef _master;
            private readonly IActorRef[] _childProcesses;
            private readonly IActorRef _saveGameManager;
            private readonly IActorRef _objectManager;

            public LoadCoordinator(IActorRef saveGameManager, IActorRef objectManager, IActorRef master, IActorRef[] childProcesses)
            {
                _saveGameManager = saveGameManager;
                _objectManager = objectManager;
                _master = master;
                _childProcesses = childProcesses;

                InitFsm();
            }

            private void InitFsm()
            {
                When(LoadStep.PreInit,
                     evt =>
                     {
                         if (evt.FsmEvent is not GameSetup setup) return Stay();

                         foreach (var module in setup.Modules)
                             setup = module.Enrich(setup);

                         if (_childProcesses.Length == 0)
                         {
                             Self.Tell(setup);
                             return GoTo(LoadStep.InitObjectManager);
                         }

                         var msg = new PreInitStage(Context.System.GetExtension<GameCore>());
                         var self = Self;

                         Task.WhenAll(_childProcesses.Select(c => c.Ask(msg)))
                             .ContinueWith(_ => self.Tell(setup));

                         return GoTo(LoadStep.InitObjectManager);
                     });

                When(LoadStep.InitObjectManager,
                     evt =>
                     {
                         if (evt.FsmEvent is not GameSetup gameSetup) return Stay();

                         _objectManager.Tell(gameSetup);
                         return GoTo(LoadStep.AppySaveGame);
                     });

                When(LoadStep.AppySaveGame,
                     evt =>
                     {
                         if (evt.FsmEvent is not GameSetup gameSetup) return Stay();

                         if (string.IsNullOrEmpty(gameSetup.SaveGame)) 
                             Self.Tell(evt.FsmEvent);
                         else
                            _saveGameManager.Tell(gameSetup);

                         return GoTo(LoadStep.SetupMessages);
                     });

                When(LoadStep.SetupMessages,
                     evt =>
                     {
                         if (evt.FsmEvent is not GameSetup gameSetup) return Stay();

                         foreach (var message in gameSetup.GameMmasterMessages) 
                             _master.Tell(message);
                         Self.Tell(gameSetup);

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

                StartWith(LoadStep.PreInit, new LoadCoordinatorState());
                Initialize();
            }

            public sealed record LoadCoordinatorState;
        }
    }

    public sealed record LoadingCompled;

    public sealed record PreInitStage(GameCore Game);
}