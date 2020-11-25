using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events.Actor
{
    [EventVersion("GameActorCreatedEvent", 1)]
    public class GameActorCreatedEvent : AggregateEvent<GameActor, GameActorId>
    {
        public GameActorCreatedEvent(PlayerType playerType, Name name, Name displayName, ActorLocation location, GameActorId actorId)
        {
            PlayerType  = playerType;
            Name        = name;
            DisplayName = displayName;
            Location    = location;
            ActorId     = actorId;
        }

        public PlayerType PlayerType { get; }

        public Name Name { get; }

        public Name DisplayName { get; }

        public ActorLocation Location { get; }

        public GameActorId ActorId { get; set; }
    }
}