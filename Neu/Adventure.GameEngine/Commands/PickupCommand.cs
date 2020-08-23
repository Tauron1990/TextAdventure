using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Commands
{
    public sealed class PickupCommand : Command<PickupCommand>
    {
        public string RoomName { get; }

        public string ItemId { get; }

        public LazyString? Respond { get; set; }

        public PickupCommand(string roomName, string itemId)
            : base(nameof(PickupCommand))
        {
            RoomName = roomName;
            ItemId = itemId;
        }
    }
}