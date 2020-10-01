using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class GameInfoCommand : GameCommand<GameInfoCommand, GameInfo, GameInfoId>
    {
        protected GameInfoCommand(GameInfoId aggregateId) : base(aggregateId)
        {
        }
    }
}