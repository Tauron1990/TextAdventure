using Akkatecture.ValueObjects;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Builder.Data.Actor
{
    public sealed class ActorLocation : SingleValueObject<RoomId>
    {
        public static readonly ActorLocation Unkowen = new ActorLocation(RoomId.FromName(new Name("Unkowen")));

        public ActorLocation(RoomId value)
            : base(value)
        {
        }
    }
}