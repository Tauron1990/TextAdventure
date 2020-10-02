using Akkatecture.Aggregates;
using Akkatecture.Specifications.Provided;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Commands.Actors;
using TextAdventures.Engine.Events;
using TextAdventures.Engine.Events.Actor;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Data.Events;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class GameActorManager : AggregateManager<GameActor, GameActorId, GameActorCommand>
    {

    }

    public sealed class GameActorState : GameState<GameActorState, GameActor, GameActorId>,
                                     IApply<GameActorCreatedEvent>
    {
        public PlayerType PlayerType { get; private set; } = PlayerType.Default;

        public Name Name { get; private set; } = Name.Default;

        public Name DisplayName { get; private set; } = Name.Default;

        public ActorLocation Location { get; private set; } = ActorLocation.Unkowen;

        public override void Hydrate(GameActorState aggregateSnapshot)
        {
            PlayerType = aggregateSnapshot.PlayerType;
            Name = aggregateSnapshot.Name;
            DisplayName = aggregateSnapshot.DisplayName;
            Location = aggregateSnapshot.Location;
        }

        public void Apply(GameActorCreatedEvent aggregateEvent)
        {
            PlayerType = aggregateEvent.PlayerType;
            Name = aggregateEvent.Name;
            DisplayName = aggregateEvent.DisplayName;
            Location = aggregateEvent.Location;
        }
    }

    [AggregateName("GameActor")]
    public sealed class GameActor : GameAggregate<GameActor, GameActorId, GameActorState>,
                                IExecute<CreateGameActorCommand>
    {
        public GameActor(GameActorId id) 
            : base(id)
        {
        }

        public bool Execute(CreateGameActorCommand command)
        {
            if (AggregateIsNewSpecification.Instance.IsSatisfiedBy(this))
            { 
                Emit(new GameActorCreatedEvent(command.PlayerType, command.Name, command.DisplayName, command.Location, Id));
                if(command.PlayerType.Value == Player.Player)
                    Emit(new PlayerCreated(Id));
            }
            return true;
        }
    }
}