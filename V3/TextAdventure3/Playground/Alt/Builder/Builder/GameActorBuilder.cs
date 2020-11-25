using JetBrains.Annotations;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;

namespace TextAdventures.Builder.Builder
{
    [PublicAPI]
    public sealed class GameActorBuilder
    {
        public GameActorBuilder(PlayerType playerType, Name name, Name displayName, ActorLocation location)
        {
            PlayerType  = playerType;
            Name        = name;
            DisplayName = displayName;
            Location    = location;
        }

        public PlayerType PlayerType { get; }

        public Name Name { get; }

        public Name DisplayName { get; }

        public ActorLocation Location { get; }
    }
}