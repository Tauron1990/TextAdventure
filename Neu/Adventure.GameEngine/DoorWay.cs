using JetBrains.Annotations;

namespace Adventure.GameEngine
{
    [PublicAPI]
    public interface IDoorway
    {
        public string TargetRoom { get; }

        public string? UnlockEvent { get; set; }

        public Direction Direction { get; }
    }

    public sealed class DoorWayConnection : IDoorway
    {
        public DoorWay Original { get; }

        public Direction Direction { get; }

        public string TargetRoom { get; }

        string? IDoorway.UnlockEvent
        {
            get => Original.UnlockEvent;
            set => Original.UnlockEvent = value;
        }

        public DoorWayConnection(DoorWay original, Direction direction, string targetRoom)
        {
            Original = original;
            Direction = direction;
            TargetRoom = targetRoom;
        }
    }

    [PublicAPI]
    public sealed class DoorWay : IDoorway
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
