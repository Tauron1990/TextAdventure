using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Builder.CommandData
{
    public sealed class CommandModifaction<TReturn, TCommand, TEcentSource> : IEventable<TEcentSource, TCommand>
        where TCommand : Command where TEcentSource : IWithMetadata, IEntityConfiguration
    {
        private readonly TEcentSource _eventSource;
        public TCommand Command { get; }

        public TReturn Info { get; }

        TEcentSource IEventable<TEcentSource, TCommand>.EventSource => _eventSource;

        TCommand IEventable<TEcentSource, TCommand>.EventData => Command;

        public CommandModifaction(TCommand command, TReturn info, TEcentSource eventSource)
        {
            _eventSource = eventSource;
            Command = command;
            Info = info;
        }
    }
}