using Akkatecture.ValueObjects;
using JetBrains.Annotations;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Builder.Data.Actor
{
    [PublicAPI]
    public sealed class ActorLocation : SingleValueObject<RoomId>
    {
        public static readonly ActorLocation Unkowen = new(RoomId.FromName(new Name("Unkowen")));

        public ActorLocation(RoomId value)
            : base(value) { }
    }
}