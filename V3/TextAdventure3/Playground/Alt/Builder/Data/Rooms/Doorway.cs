using System.Collections.Generic;
using Akkatecture.Entities;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data.Rooms
{
    [PublicAPI]
    public sealed class Doorway : Entity<RoomId>
    {
        public Doorway(RoomId id, Name target, LockState locked, Direction direction) : base(id)
        {
            Target    = target;
            Locked    = locked;
            Direction = direction;
        }

        public Name Target { get; }

        public LockState Locked { get; }

        public Direction Direction { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Target;
            yield return Direction;
        }

        public Doorway With(LockState state)
            => new(Id, Target, state, Direction);
    }
}