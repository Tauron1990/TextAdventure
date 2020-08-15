using Adventure.GameEngine.Interfaces;
using Adventure.TextProcessing;
using CodeProject.ObjectPool.Specialized;
using EcsRx.Executor.Handlers;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;

namespace Adventure.GameEngine.Core
{
    public sealed class GameModule : IDependencyModule
    {
        private readonly Game _game;

        public GameModule(Game game) 
            => _game = game;

        public void Setup(IDependencyContainer container)
        {
            container.Bind<IConventionalSystemHandler, DisposibleSystemHandler>();
            container.Bind<Game>(b => b.ToInstance(_game));
            container.Bind<IDiceRoll, DiceRoll>(b => b.AsSingleton());
            container.Bind<Parser>(b => b.ToInstance(new Parser()));
            container.Bind<IStringBuilderPool>(b => b.ToInstance(new StringBuilderPool()));
        }
    }
}