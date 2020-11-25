using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Commands;
using TextAdventures.Builder.Data.Querys;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public sealed class GameMaster
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef   _cooredinator;

        public GameMaster(IActorRef cooredinator, ActorSystem actorSystem)
        {
            _cooredinator = cooredinator;
            _actorSystem  = actorSystem;
        }

        public Task WhenTerminated => _actorSystem.WhenTerminated;

        public Task Stop()
            => _actorSystem.Terminate().ContinueWith(_ => _actorSystem.Dispose());

        public void SendCommand(IGameCommand command)
            => _cooredinator.Tell(command);

        public Task<QueryResult> SendQuery(IGameQuery query)
            => _cooredinator.Ask<QueryResult>(query);

        public Task<QueryResult> SendSave(SaveGameCommand query)
            => _cooredinator.Ask<QueryResult>(query);

        public void Add(INewProjector projector)
            => _cooredinator.Tell(projector);

        public void Add(INewAggregate aggregate)
            => _cooredinator.Tell(aggregate);
    }
}