using Akkatecture.Aggregates;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Events
{
    public sealed class ActorRoomChanged : AggregateEvent<GameActor, GameActorId>
    {
        public GameActorId Actor { get; }

        public RoomId NewRoom { get; }

        public ActorRoomChanged(GameActorId actor, RoomId newRoom)
        {
            Actor = actor;
            NewRoom = newRoom;
        }
    }
}