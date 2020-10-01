using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public sealed class CommandLogCommand : GameCommand<CommandLogCommand, CommandLog, CommandLogId>
    {
        public ILogCommand Command { get; }


        public CommandLogCommand(ILogCommand command) 
            : base(CommandLogId.Id)
        {
            Command = command;
        }
    }
}