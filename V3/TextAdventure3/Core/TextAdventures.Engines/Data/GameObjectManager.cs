using System.Threading.Tasks;
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

        public Task<GameObject?> GetObject(string name)
            => _objectManager
              .Ask<RespondGameObject>(new RequestGameObject(name))
              .ContinueWith(t => t.IsCompletedSuccessfully ? t.Result.GameObject : null);

        public Task<TType?> GetGlobalComponent<TType>()
            where TType : class
            => _objectManager
              .Ask<RespondGameComponent>(new RequestGameComponent(typeof(TType)))
              .ContinueWith(t => t.IsCompletedSuccessfully ? t.Result.Type as TType : null);
    }
}