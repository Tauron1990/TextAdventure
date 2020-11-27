using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.CommandSystem;

namespace TextAdventures.Engine.Data
{
    [PublicAPI]
    public sealed class GameObjectManager
    {
        private readonly IActorRef _objectManager;

        public GameObjectManager(IActorRef objectManager) => _objectManager = objectManager;

        public void Dispatch(IGameCommand command)
            => _objectManager.Tell(command);
    }
}