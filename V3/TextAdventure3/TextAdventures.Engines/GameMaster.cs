using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Querys;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Querys;
using TextAdventures.Engine.Querys;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public sealed class GameMaster
    {
        private readonly IActorRef _cooredinator;
        private readonly ActorSystem _actorSystem;

        public GameMaster(IActorRef cooredinator, ActorSystem actorSystem)
        {
            _cooredinator = cooredinator;
            _actorSystem = actorSystem;
        }

        public Task Stop()
            => _actorSystem.Terminate().ContinueWith(_ => _actorSystem.Dispose());

        public void SendCommand(IGameCommand command)
            => _cooredinator.Tell(command);

        public Task<QueryResult> SendQuery(IGameQuery query)
            => _cooredinator.Ask<QueryResult>(query);

        public void Add(INewProjector projector)
            => _cooredinator.Tell(projector);

        public void Add(INewAggregate aggregate)
            => _cooredinator.Tell(aggregate);
    }
}