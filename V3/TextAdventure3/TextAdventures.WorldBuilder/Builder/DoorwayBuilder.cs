using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Builder.Builder
{
    public sealed class DoorwayBuilder
    {
        public LockState LockState { get; }

        public Name Target { get; }

        public Direction Direction { get; }

        public DoorwayBuilder(LockState lockState, Name target, Direction direction)
        {
            LockState = lockState;
            Target = target;
            Direction = direction;
        }
    }
}