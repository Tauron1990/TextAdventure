using Adventure.GameEngine.Commands;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.CommandData
{
    public delegate void CommandHandler(ICommandProperties command);

    public sealed class DelegateCommand : Command<DelegateCommand>
    {
        private readonly object? _parameter;
        private ICommandProperties? _commandProperties;

        public DelegateCommand(object? parameter, CommandHandler actualHandler) 
            : base(nameof(DelegateCommand))
        {
            _parameter = parameter;
            ActualHandler = actualHandler;
        }

        internal CommandHandler ActualHandler { get; }

        internal void UpdatePropertys(ICommandProperties properties)
            => _commandProperties = properties;

        [PublicAPI]
        public object? Parameter
        {
            get
            {
                if (_parameter is IParameterFactory factory && _commandProperties != null)
                    return factory.Create(_commandProperties);
                return _parameter;
            }
        }
    }
}