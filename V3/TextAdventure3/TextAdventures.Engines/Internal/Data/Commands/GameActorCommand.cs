using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Commands;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class GameActorCommand : GameCommand<GameActorCommand, Aggregates.GameActor, GameActorId>
    {
        protected GameActorCommand(GameActorId aggregateId) 
            : base(aggregateId)
        {
        }
    }
}