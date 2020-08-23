using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Collections;
using EcsRx.Events;

namespace Adventure.GameEngine.Systems
{
    public sealed class PickupProcessor : CommandProcessor<PickupCommand>
    {
        public PickupProcessor(IEventSystem eventSystem, IObservableGroupManager manager)
            : base(eventSystem, manager)
        {
        }

        protected override void ProcessCommand(PickupCommand command)
        {
            var evt = new TransferObjectToInventory(command.ItemId, nameof(PickupCommand));
            EventSystem.Publish(evt);
            UpdateTextContent(string.IsNullOrWhiteSpace(command.Respond) ? evt.Result : LazyString.New(command.Respond));
        }
    }
}