using Akkatecture.Aggregates;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Internal.Data.Events
{
    public sealed class CommandToLogAddedEvent : AggregateEvent<CommandLog, CommandLogId>
    {
        public CommandToLogAddedEvent(ILogCommand command) => Command = command;
        public ILogCommand Command { get; }
    }
}