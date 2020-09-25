using System;
using System.Collections.Generic;
using Akka.Actor;
using Tauron.Akka;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Internal.Querys;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class GameMasterActor : ExposedReceiveActor
    {
        private readonly Dictionary<Type, (Props Props, string Name)> _aggregates = new Dictionary<Type, (Props Props, string Name)>();
        private IActorRef _projector = ActorRefs.Nobody;

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
            Receive<IAddAggregate>(a => _aggregates[a.Target] = (a.Props, a.Name));

            Receive<StartGame>(InitializeGame);
        }

        private void InitializeGame(StartGame start)
        {
            _projector = Context.ActorOf<ProjectionManager>("ProjectorManager");
            _projector.Tell(start);

            Self.Tell(new AddAggregate<RoomManager, Room, RoomId, RoomCommand>());

            //if(start.NewGame)

        }
    }
}