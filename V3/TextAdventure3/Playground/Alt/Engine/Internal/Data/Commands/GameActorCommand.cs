using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class GameActorCommand : GameCommand<GameActorCommand, GameActor, GameActorId>
    {
        protected GameActorCommand(GameActorId aggregateId)
            : base(aggregateId) { }
    }
}