using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.Internal.Data.Commands;

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
    }
}