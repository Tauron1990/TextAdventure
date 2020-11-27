using System;
using Akka.Actor;
using TextAdventures.Engine.EventSystem;

namespace TextAdventures.Engine.Data
{
    public sealed class GameCore : IExtension
    {
        public EventDispatcher EventDispatcher { get; }

        public GameObjectManager ObjectManager { get; }

        public GameCore(EventDispatcher eventDispatcher, GameObjectManager objectManager)
        {
            EventDispatcher = eventDispatcher;
            ObjectManager = objectManager;
        }

        public static GameCore Get(ActorSystem system)
            => system.GetExtension<GameCore>();

        internal sealed class GameCoreId : ExtensionIdProvider<GameCore>
        {
            private readonly EventDispatcher _eventDispatcher;

            private readonly GameObjectManager _objectManager;

            public GameCoreId(EventDispatcher eventDispatcher, GameObjectManager objectManager)
            {
                _eventDispatcher = eventDispatcher;
                _objectManager = objectManager;
            }

            public override GameCore CreateExtension(ExtendedActorSystem system) => new(_eventDispatcher, _objectManager);
        }
    }
}