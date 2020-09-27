using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;

namespace TextAdventures.Builder.Builder
{
    public sealed class ActorBuilder
    {
        public PlayerType PlayerType { get; }

        public Name Name { get; }

        public Name DisplayName { get; }

        public ActorLocation Location { get; }

        public ActorBuilder(PlayerType playerType, Name name, Name displayName, ActorLocation location)
        {
            PlayerType = playerType;
            Name = name;
            DisplayName = displayName;
            Location = location;
        }
    }
}