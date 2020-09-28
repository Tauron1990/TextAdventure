using System.Collections.Immutable;
using TextAdventures.Builder.Data.Command;

namespace TextAdventures.Engine.Events
{
    public sealed class CommandsUpdatedEvent : TransistentEvent<CommandsUpdatedEvent>
    {
        public ImmutableList<IGameCommand> Commands { get; }

        public CommandsUpdatedEvent(ImmutableList<IGameCommand> commands)
            => Commands = commands;
    }
}