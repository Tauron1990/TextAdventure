using Akka.Actor;
using Akkatecture.Aggregates;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events
{
    public sealed class PlayerCreated : AggregateEvent<GameActor, GameActorId>
    {
        public GameActorId Actor { get; }

        public PlayerCreated(GameActorId actor)
            => Actor = actor;
    }
}