using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Infrastructure.Dependencies;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class CommandBuilder
    {
        private readonly IDependencyContainer _container;
        private readonly GameConfiguration _configuration;

        public CommandBuilder(IDependencyContainer container, GameConfiguration configuration)
        {
            _container = container;
            _configuration = configuration;
        }

        public static Command Create(CommandHandler handler, object? parameter = null)
            => new DelegateCommand(parameter, handler);

        public static Command Create(CommandHandler handler, IParameterFactory? parameter = null)
            => new DelegateCommand(parameter, handler);

        public static Command Create(string id, CommandHandler handler, object? parameter = null)
            => new DelegateCommand(id, parameter, handler);

        public static Command Create(string id, CommandHandler handler, IParameterFactory? parameter = null)
            => new DelegateCommand(id, parameter, handler);

        public GenericCommandBuilder WithGenric(string name, Command command)
        {
            var print = new GenericCommandBlueprint(command, name);
            _configuration.Entities.Add(new SimpleBlueprintProvider(print));
            return new GenericCommandBuilder(print);
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