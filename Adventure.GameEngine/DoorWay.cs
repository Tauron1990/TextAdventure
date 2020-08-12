using JetBrains.Annotations;

namespace Adventure.GameEngine
{
    public sealed class DoorWayConnection
    {
        public DoorWay Original { get; }

        public Direction Direction { get; }

        public string To { get; }

        public DoorWayConnection(DoorWay original, Direction direction, string to)
        {
            Original = original;
            Direction = direction;
            To = to;
        }
    }

    [PublicAPI]
    public sealed class DoorWay
    {
        public string TargetRoom { get; }

        public string? UnlockEvent { get; set; }

        public Direction Direction { get; }

        public DoorWay(string targetRoom, string? unlockEvent, Direction direction)
        {
            TargetRoom = targetRoom;
            UnlockEvent = unlockEvent;
            Direction = direction;
        }
    }
}
