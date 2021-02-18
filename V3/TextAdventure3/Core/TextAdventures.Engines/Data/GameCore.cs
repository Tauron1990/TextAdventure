using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.EventSystem;

namespace TextAdventures.Engine.Data
{
    [PublicAPI]
    public sealed record GameCore(EventDispatcher EventDispatcher, GameObjectManager ObjectManager, GameMaster GameMaster) : IExtension
    {
        public static GameCore Get(ActorSystem system)
            => system.GetExtension<GameCore>();

        internal sealed class GameCoreId : ExtensionIdProvider<GameCore>
        {
            private readonly EventDispatcher _eventDispatcher;
            private readonly GameMaster _gameMaster;
            private readonly GameObjectManager _objectManager;

            public GameCoreId(EventDispatcher eventDispatcher, GameObjectManager objectManager, GameMaster gameMaster)
            {
                _eventDispatcher = eventDispatcher;
                _objectManager = objectManager;
                _gameMaster = gameMaster;
            }

            public override GameCore CreateExtension(ExtendedActorSystem system)
                => new(_eventDispatcher, _objectManager, _gameMaster);
        }
    }
}