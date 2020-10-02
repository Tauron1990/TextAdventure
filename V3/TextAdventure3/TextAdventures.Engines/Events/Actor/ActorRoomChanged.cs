using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Actor
{
    [EventVersion("ActorRoomChanged", 1)]
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