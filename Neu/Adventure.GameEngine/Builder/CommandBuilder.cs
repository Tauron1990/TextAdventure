using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Blueprints;
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
        private readonly GameConfiguration _configuration;

        public CommandBuilder(IDependencyContainer container, IInternalGameConfiguration internalGameConfiguration, GameConfiguration configuration)
        {
            _container = container;
            _internalGameConfiguration = internalGameConfiguration;
            _configuration = configuration;
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

        public GenericCommandBuilder WithGenric(string name, Command command)
        {
            var print = new GenericCommandBlueprint(command, name);
            _configuration.Entities.Add(new SimpleBlueprintProvider(print));
            return new GenericCommandBuilder(_internalGameConfiguration.RegisterCommand(command), print);
        }

        public GenericCommandBuilder WithGenric(string name, CommandHandler handler, object? parameter = null)
        {
            var del =new DelegateCommand(name, parameter, handler);
            return WithGenric(name, del);
        }

        public GenericCommandBuilder WithGenric(string name, CommandHandler handler, IParameterFactory? parameter = null)
        {
            var del = new DelegateCommand(name, parameter, handler);
            return WithGenric(name, del);
        }
    }
}