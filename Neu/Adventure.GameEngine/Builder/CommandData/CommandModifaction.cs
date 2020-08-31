using System.Collections.Generic;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Builder.CommandData
{
    public sealed class CommandModifaction<TReturn, TCommand, TEcentSource> : IEventable<TEcentSource, TCommand>, IWithMetadata, ITargetValue
        where TCommand : Command where TEcentSource : IWithMetadata, IEntityConfiguration
    {
        private readonly TEcentSource _eventSource;
        private readonly Dictionary<string, object> _metadata;

        public TCommand Command { get; }

        public TReturn Info { get; }

        TEcentSource IEventable<TEcentSource, TCommand>.EventSource => _eventSource;

        TCommand IEventable<TEcentSource, TCommand>.EventData => Command;

        public CommandModifaction(TCommand command, TReturn info, TEcentSource eventSource, Dictionary<string, object> metadata)
        {
            _eventSource = eventSource;
            _metadata = metadata;
            Command = command;
            Info = info;
        }

        Dictionary<string, object> IWithMetadata.Metadata => _metadata;

        object ITargetValue.Value => throw new System.NotImplementedException();
    }
}