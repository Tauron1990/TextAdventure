using System;
using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.EventSystem;

namespace TextAdventures.Engine.Data
{
    [PublicAPI]
    public sealed class GameCore : IExtension
    {
        public EventDispatcher EventDispatcher { get; }

        public GameObjectManager ObjectManager { get; }

        public GameMaster GameMaster { get; }

        private GameCore(EventDispatcher eventDispatcher, GameObjectManager objectManager, GameMaster gameMaster)
        {
            EventDispatcher = eventDispatcher;
            ObjectManager = objectManager;
            GameMaster = gameMaster;
        }

        public static GameCore Get(ActorSystem system)
            => system.GetExtension<GameCore>();

        internal sealed class GameCoreId : ExtensionIdProvider<GameCore>
        {
            private readonly EventDispatcher _eventDispatcher;
            private readonly GameObjectManager _objectManager;
            private readonly GameMaster _gameMaster;

            public GameCoreId(EventDispatcher eventDispatcher, GameObjectManager objectManager, GameMaster gameMaster)
            {
                _eventDispatcher = eventDispatcher;
                _objectManager = objectManager;
                _gameMaster = gameMaster;
            }

            public override GameCore CreateExtension(ExtendedActorSystem system) => new(_eventDispatcher, _objectManager, _gameMaster);
        }
    }
}