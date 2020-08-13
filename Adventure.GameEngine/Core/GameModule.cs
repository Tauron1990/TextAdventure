using Adventure.GameEngine.Interfaces;
using Adventure.TextProcessing;
using EcsRx.Infrastructure.Dependencies;

namespace Adventure.GameEngine.Core
{
    public sealed class GameModule : IDependencyModule
    {
        private readonly Game _game;

        public GameModule(Game game) 
            => _game = game;

        public void Setup(IDependencyContainer container)
        {
            container.Bind(typeof(Parser), typeof(Parser), new BindingConfiguration{ AsSingleton = true });
            container.Bind(typeof(IDiceRoll), typeof(DiceRoll), new BindingConfiguration { AsSingleton = true });
            container.Bind(typeof(Game), typeof(Game), new BindingConfiguration { AsSingleton = true, ToInstance = _game });
        }
    }
}