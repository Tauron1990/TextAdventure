using Akkatecture.Commands;
using JetBrains.Annotations;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class GameInfoCommand : GameCommand<GameInfoCommand, GameInfo, GameInfoId>
    {
        protected GameInfoCommand(GameInfoId aggregateId) : base(aggregateId)
        {
        }

        protected GameInfoCommand(GameInfoId aggregateId, [NotNull] CommandId sourceId) : base(aggregateId, sourceId)
        {
        }
    }
}