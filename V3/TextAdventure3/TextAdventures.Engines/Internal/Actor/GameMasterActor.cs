using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Tauron.Akka;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Builder.Querys;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Events;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Internal.WorldConstructor;
using TextAdventures.Engine.Processors.Commands;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class GameMasterActor : ExposedReceiveActor
    {
        private readonly Action<Exception> _failAction;
        private readonly Dictionary<Type, (Props Props, string Name)> _aggregates = new Dictionary<Type, (Props Props, string Name)>();

        private IActorRef _projector = ActorRefs.Nobody;
        private IActorRef _loadingManager = ActorRefs.Nobody;
        private IActorRef _updateManager = ActorRefs.Nobody;
        private IActorRef _saveGameManager = ActorRefs.Nobody;

        public GameMasterActor(Action<Exception> failAction)
        {
            _failAction = failAction;
            Receive<IGameQuery>(q => _projector.Forward(q));
            Receive<IGameCommand>(c =>
            {
                if (!_aggregates.TryGetValue(c.Target, out var target)) return false;
                
                Context.GetOrAdd(target.Name, target.Props).Tell(c);
                return true;

            });
            
            Receive<INewProjector>(p => _projector.Forward(p));
            Receive<INewQueryHandler>(h => _projector.Forward(h));
            Receive<INewAggregate>(a => _aggregates[a.Target] = (a.Props, a.Name));
            Receive<INewSaga>(s => Context.ActorOf(s.SagaManager, s.Name));

            Receive<RegisterForUpdate>(r => _updateManager.Forward(r));
            Receive<SaveGameCommand>(c => _saveGameManager.Forward(c));

            ReceiveAsync<StartGame>(InitializeGame);
            Receive<LoadingCompled>(GameLoadingCompled);
            Receive<Task<IDisposable>>(s => s.Result.Dispose());
        }

        private void GameLoadingCompled(LoadingCompled obj) 
            => new GameLoaded(obj.Info, new GameMaster(Self, Context.System)).Publish(Context.System.EventStream);

        private async Task InitializeGame(StartGame start)
        {
            _loadingManager = Context.ActorOf<LoadingManagerActor>("LoadingManager");
            var waiter = LoadingSequence.Add(_loadingManager);
            _saveGameManager = Context.ActorOf(Props.Create<SaveGameManager>(), "SaveGameManager");

            try
            {
                var result = await _saveGameManager.Ask<QueryResult>(start);
                if (result is QueryFailed)
                {
                    #pragma warning disable 4014
                    Context.System.Terminate();
                    #pragma warning restore 4014
                    return;
                }

                foreach (var (name, prop) in start.World.ActorProps) 
                    Context.ActorOf(prop, name);

                _loadingManager.Tell(WaitUntilLoaded.New(Self, new LoadingCompled(start.SaveGame)));

                _projector = Context.ActorOf(() => new ProjectionManagerActor(_loadingManager), "ProjectorManager");
                _projector.Tell(start);

                Self.Tell(NewAggregate<RoomManager, Room, RoomId, RoomCommand>.Create());
                Self.Tell(NewAggregate<GameActorManager, GameActor, GameActorId, GameActorCommand>.Create());
                Self.Tell(NewAggregate<GameInfoManager, GameInfo, GameInfoId, GameInfoCommand>.Create());

                Self.Tell(NewSaga.Create<CommandTrackerManager, CommandTracker, CommandTrackerId, CommandTrackerLocator>().With(Self));

                if (start.NewGame)
                    new WorldBuilder(start.World, Self).Construct();
                else
                    new WorldBuilder(start.World, Self).Load();

                _updateManager = Context.ActorOf<UpdateManagerActor>();
            }
            finally
            {
                await Task.Delay(2000);
                Self.Tell(waiter);
            }

        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(e =>
            {
                _failAction(e);
                return Directive.Escalate;
            });
        }

        private sealed class LoadingCompled
        {
            public SaveProfile Info { get; }

            public LoadingCompled(SaveProfile info) => Info = info;
        }
    }
}