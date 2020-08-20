using System;
using System.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Events;
using Adventure.GameEngine.Querys;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class InteractionManager : MultiEventReactionSystem
    {
        private readonly Func<IEntity> _currentRoom;

        public InteractionManager(IEventSystem eventSystem, Game game)
            : base(eventSystem)
        {
            var ent = game.EntityDatabase.GetCollection();

            _currentRoom = () => ent.Query(new QueryCurrentRoom()).First();
        }

        public override IGroup Group { get; } = new Group(typeof(InteractiveObject), typeof(IngameObject));

        protected override void Init()
        {
            Receive<TransferObjectToInventory>(TransferObject);
        }

        private void TransferObject(TransferObjectToInventory obj)
        {
            var currentRoom = _currentRoom();
            var roomName = currentRoom.GetComponent<Room>().Name;

            var gameObject = ObservableGroup.Query(new QueryNamedItemFromRoom(roomName, obj.Id)).FirstOrDefault();
            if (gameObject == null)
                obj.Result = new LazyString(GameConsts.NoObjectForPickupFound).AddParameters(obj.Id);
            {
                if (gameObject.GetComponent<InteractiveObject>().TargetAction != obj.Codes)
                {
                    obj.Result = new LazyString(GameConsts.ObjectUnaleToPickup).AddParameters(obj.Id);
                }
                else
                {
                    gameObject.GetComponent<IngameObject>().Location.Value = GameConsts.PlayerInventoryId;
                    obj.Result = new LazyString(GameConsts.ObjectPickedUp).AddParameters(obj.Id);
                }
            }
        }
    }
}