using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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

        public IObservable<GameObject?> GetObject(string name)
            => _objectManager
              .Ask<RespondGameObject>(new RequestGameObject(name))
              .ToObservable().Select(r => r.GameObject);

        public IObservable<TType?> GetGlobalComponent<TType>()
            where TType : class
            => _objectManager
              .Ask<RespondGameComponent>(new RequestGameComponent(typeof(TType)))
              .ToObservable().Select(r => r.Component as TType);
    }
}