using System.Threading.Tasks;
using Akka.Actor;

namespace TextAdventures.Engine
{
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
    }
}