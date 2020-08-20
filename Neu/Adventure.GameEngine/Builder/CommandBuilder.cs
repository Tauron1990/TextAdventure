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

        public CommandBuilder(IDependencyContainer container)
            => _container = container;

        public CommandBuilder RegisterCommand<TCommand, TProcessor>()
            where TProcessor : CommandProcessor<TCommand> 
            where TCommand : Command<TCommand>
        {
            _container.Bind<ISystem, TProcessor>();
            return this;
        }
    }
}