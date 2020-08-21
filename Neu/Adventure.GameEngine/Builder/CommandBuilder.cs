using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Systems;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class CommandBuilder
    {
        private readonly IDependencyContainer _container;
        private readonly IInternalGameConfiguration _internalGameConfiguration;

        public CommandBuilder(IDependencyContainer container, IInternalGameConfiguration internalGameConfiguration)
        {
            _container = container;
            _internalGameConfiguration = internalGameConfiguration;
        }

        public CommandId With<TCommand, TProcessor>(TCommand command)
            where TProcessor : CommandProcessor<TCommand> 
            where TCommand : Command<TCommand>
        {
            _container.Bind<ISystem, TProcessor>();
            return _internalGameConfiguration.RegisterCommand(command);
        }

        public CommandId With<TCommand, TProcessor>()
            where TProcessor : CommandProcessor<TCommand>
            where TCommand : Command<TCommand>, new()
        {
            _container.Bind<ISystem, TProcessor>();
            return _internalGameConfiguration.RegisterCommand(new TCommand());
        }

        public CommandId With(CommandHandler handler, object? parameter = null)
            => _internalGameConfiguration.RegisterCommand(new DelegateCommand(parameter, handler));

        public CommandId With(CommandHandler handler, IParameterFactory? parameter = null)
            => _internalGameConfiguration.RegisterCommand(new DelegateCommand(parameter, handler));

        public CommandId With(string id, CommandHandler handler, object? parameter = null)
            => _internalGameConfiguration.RegisterCommand(new DelegateCommand(id, parameter, handler));

        public CommandId With(string id, CommandHandler handler, IParameterFactory? parameter = null)
            => _internalGameConfiguration.RegisterCommand(new DelegateCommand(id, parameter, handler));
    }
}