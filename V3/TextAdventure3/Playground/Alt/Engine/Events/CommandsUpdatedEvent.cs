using System.Collections.Immutable;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Command;

namespace TextAdventures.Engine.Events
{
    public sealed class CommandsUpdatedEvent : TransistentEvent<CommandsUpdatedEvent>
    {
        public CommandsUpdatedEvent(ImmutableList<IGameCommand> commands)
            => Commands = commands;

        public ImmutableList<IGameCommand> Commands { get; }
    }
}