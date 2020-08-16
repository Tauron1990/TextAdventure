using System;
using System.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using Adventure.GameEngine.Querys;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;

namespace Adventure.GameEngine.Systems
{
    public sealed class InteractionManager : MultiEventReactionSystem
    {
        private readonly Func<IEntity> _currentRoom;

        private readonly Func<IEntity> _player;

        public InteractionManager(IEventSystem eventSystem, Game game)
            : base(eventSystem)
        {
            var ent = game.EntityDatabase.GetCollection();

            _currentRoom = () => ent.Query(new QueryCurrentRoom()).First();
            _player = () => ent.Query(new QueryPlayer()).First();
        }

        public override IGroup Group { get; } = new Group(typeof(InteractiveObject), typeof(IngameObject));

        protected override void Init()
        {
            Receive<TransferObjectToInventory>(TransferObject);
        }

        private void TransferObject(TransferObjectToInventory obj)
        {
            var player = _player();
            var currentRoom = _currentRoom();
            var roomName = currentRoom.GetComponent<Room>().Name;

            var objects = ObservableGroup.Query(new QueryNamedItemFromRoom(roomName, obj.Id));
        }
    }
}