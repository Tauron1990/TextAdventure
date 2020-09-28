using System;
using System.Collections.Generic;
using System.Threading;
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
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Internal.WorldConstructor;
using TextAdventures.Engine.Processors.Commands;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class GameMasterActor : ExposedReceiveActor
    {
        private readonly Dictionary<Type, (Props Props, string Name)> _aggregates = new Dictionary<Type, (Props Props, string Name)>();
        private IActorRef _projector = ActorRefs.Nobody;
        private IActorRef _loadingManager = ActorRefs.Nobody;
        private IActorRef _updateManager = ActorRefs.Nobody;

        public GameMasterActor()
        {
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

            Receive<StartGame>(InitializeGame);
            Receive<LoadingCompled>(GameLoadingCompled);
            Receive<Task<IDisposable>>(s => s.Result.Dispose());
        }

        private void GameLoadingCompled(LoadingCompled obj) 
            => new GameLoaded().Publish(Context.System.EventStream);

        private void InitializeGame(StartGame start)
        {
            _loadingManager = Context.ActorOf<LoadingManagerActor>("LoadingManager");
            var waiter = LoadingSequence.Add(_loadingManager);

            try
            {
                _loadingManager.Tell(WaitUntilLoaded.New(Self, LoadingCompled.Instance));

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
                Thread.Sleep(2000);
                Self.Tell(waiter);
            }

        }

        private sealed class LoadingCompled
        {
            public static readonly LoadingCompled Instance = new LoadingCompled();
        }
    }
}