using Akkatecture.Commands;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Actors
{
    public class CreateGameActorCommand : GameActorCommand
    {
        public PlayerType PlayerType { get; }

        public Name Name { get; }

        public Name DisplayName { get; }

        public ActorLocation Location { get; }

        public CreateGameActorCommand(GameActorId aggregateId, PlayerType playerType, Name name, Name displayName, ActorLocation location) 
            : base(aggregateId)
        {
            PlayerType = playerType;
            Name = name;
            DisplayName = displayName;
            Location = location;
        }

        public CreateGameActorCommand(GameActorId aggregateId, CommandId sourceId, PlayerType playerType, Name name, Name displayName, ActorLocation location) 
            : base(aggregateId, sourceId)
        {
            PlayerType = playerType;
            Name = name;
            DisplayName = displayName;
            Location = location;
        }
    }
}