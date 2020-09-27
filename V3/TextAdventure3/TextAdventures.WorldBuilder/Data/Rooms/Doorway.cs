using System.Collections.Generic;
using Akkatecture.Entities;

namespace TextAdventures.Builder.Data.Rooms
{
    public sealed class Doorway : Entity<RoomId>
    {
        public Name Target { get; }

        public LockState Locked { get; }

        public Direction Direction { get; }

        public Doorway(RoomId id, Name target, LockState locked, Direction direction) : base(id)
        {
            Target = target;
            Locked = locked;
            Direction = direction;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Target;
            yield return Direction;
        }

        public Doorway With(LockState state)
            => new Doorway(Id, Target, state, Direction);
            
    }
}