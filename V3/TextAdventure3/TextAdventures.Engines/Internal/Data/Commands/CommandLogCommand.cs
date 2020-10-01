using Akkatecture.Commands;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public sealed class CommandLogCommand : GameCommand<CommandLogCommand, CommandLog, CommandLogId>
    {
        public CommandLogCommand(CommandLogId aggregateId) 
            : base(aggregateId)
        {
        }

        public CommandLogCommand(CommandLogId aggregateId, CommandId sourceId) 
            : base(aggregateId, sourceId)
        {
        }
    }
}