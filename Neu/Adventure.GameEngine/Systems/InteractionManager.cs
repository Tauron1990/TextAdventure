using System.Linq;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Querys;
using Adventure.GameEngine.Systems.Components;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class InteractionManager : MultiEventReactionSystem
    {
        private readonly CurrentRoom _currentRoom;

        public InteractionManager(IEventSystem eventSystem, Game game)
            : base(eventSystem)
            => _currentRoom = new CurrentRoom(game.ObservableGroupManager);

        public override IGroup Group { get; } = new Group(typeof(InteractiveObject), typeof(IngameObject));

        protected override void Init()
        {
            Receive<TransferObjectToInventory>(TransferObject);
        }

        private void TransferObject(TransferObjectToInventory obj)
        {
            var currentRoom = _currentRoom.Value;
            var roomName = currentRoom.GetComponent<Room>().Name;

            var gameObject = ObservableGroup.Query(new QueryNamedItemFromRoom(roomName, obj.Id)).FirstOrDefault();
            if (gameObject == null)
                obj.Result = LazyString.New(GameConsts.NoObjectForPickupFound).AddParameters(obj.Id);
            {
                if (gameObject.GetComponent<InteractiveObject>().TargetCommand != obj.TargetCommand)
                {
                    obj.Result = LazyString.New(GameConsts.ObjectUnaleToPickup).AddParameters(obj.Id);
                }
                else
                {
                    gameObject.GetComponent<IngameObject>().Location.Value = GameConsts.PlayerInventoryId;
                    obj.Result = LazyString.New(GameConsts.ObjectPickedUp).AddParameters(obj.Id);
                }
            }
        }
    }
}