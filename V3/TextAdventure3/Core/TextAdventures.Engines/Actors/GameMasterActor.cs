using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Tauron;
using Tauron.Features;
using TextAdventures.Builder;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.EventSystem;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine.Actors
{
    internal sealed class GameMasterActor : GameProcess<GameMasterActor.GmState>
    {
        public sealed record GmState(bool IsRunning, Action<Exception> ErrorHandler);

        public static IPreparedFeature Create(GameProfile profile)
            => Feature.Create(() => new GameMasterActor(profile), () => new GmState(false, _ => {}));

        private readonly IActorRef _eventDispatcher;
        private readonly IActorRef _gameObjectManager;
        private readonly IActorRef _saveGameManager;

        private GameMasterActor(GameProfile profile)
        {
            _eventDispatcher = Context.ActorOf("EventDispatcher", EventDispatcherActor.Prefab());
            _gameObjectManager = Context.ActorOf("GameObjectManager", GameObjectManagerActor.Prefab());
            _saveGameManager = Context.ActorOf("SaveGameManagerActor", SaveGameManagerActor.Prefab(profile, _gameObjectManager));
        }

        protected override void Config()
        {
            Context.System.RegisterExtension(new GameCore.GameCoreId(
                new EventDispatcher(_eventDispatcher),
                new GameObjectManager(_gameObjectManager),
                new GameMaster(Self, Context.System)));

            Receive<GameEvent>(obs => obs.Select(o => o.Event).ForwardToActor(_eventDispatcher));
            Receive<RequestEventSource>(obs => obs.Select(o => o.Event).ForwardToActor(_eventDispatcher));

            Receive<ComponentBlueprint>(obs => obs.Select(p => p.Event).ForwardToActor(_gameObjectManager));
            Receive<GameObjectBlueprint>(obs => obs.Select(p => p.Event).ForwardToActor(_gameObjectManager));
            Receive<IGameCommand>(obs => obs.Select(p => p.Event).ForwardToActor(_gameObjectManager));
            Receive<RegisterCommandProcessorBase>(obs => obs.Select(p => p.Event).ForwardToActor(_gameObjectManager));

            Receive<MakeSaveGame>(obs => obs.Select(p => p.Event).ForwardToActor(_saveGameManager));
            

            Receive<string>(obs => 
                                obs.Where(p => !p.State.IsRunning)
                                   .Do(_ =>
                                       {
                                           var msg = new LoadingCompled(GameCore.Get(Context.System));

                                           foreach (var child in Context.GetChildren()) 
                                               child.Tell(msg);
                                       })
                                   .Select(p => p.State with{IsRunning = true}));
            Receive<GameSetup>(obs =>
                                   obs.Select(p => (p.State, p.Event, Processes:p.Event.GameProcesses.Select(gp => Context.ActorOf(gp.Key, gp.Value)).ToArray()))
                                      .Do(p => Context.ActorOf(Props.Create(() => new LoadCoordinator(_saveGameManager, _gameObjectManager, Self, p.Processes))).Tell(p.Event))
                                      .Select(p => p.State with{ErrorHandler = p.Event.Error}));
            
            var system = Context.System;
            SupervisorStrategy = new OneForOneStrategy(e =>
                                         {
                                             CurrentState.ErrorHandler(e);
                                             system.Terminate();

                                             return Directive.Stop;
                                         });
            base.Config();
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

            private readonly IActorRef[] _childProcesses;

            private readonly IActorRef _master;
            private readonly IActorRef _objectManager;
            private readonly IActorRef _saveGameManager;

            public LoadCoordinator(IActorRef saveGameManager, IActorRef objectManager, IActorRef master,
                IActorRef[] childProcesses)
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

    public sealed record LoadingCompled(GameCore GameCore);

    public sealed record PreInitStage(GameCore Game);
}