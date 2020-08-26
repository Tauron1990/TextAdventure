using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Systems.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class PickupProcessor : CommandProcessor<PickupCommand>
    {
        public PickupProcessor(Game game)
            : base(game)
        {
        }

        protected override void ProcessCommand(PickupCommand command)
        {
            var evt = new TransferObjectToInventory(command.ItemId, nameof(PickupCommand));
            EventSystem.Publish(evt);
            UpdateTextContent(command.Respond ?? evt.Result);
        }
    }
}