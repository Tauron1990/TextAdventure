using System.Collections.Generic;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Infrastructure.Dependencies;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class GameConfiguration : IWithMetadata
    {
        private readonly IDependencyContainer _container;
        private readonly Game _game;

        internal List<IBluePrintProvider> Entities { get; } = new List<IBluePrintProvider>();

        public CommandBuilder NewCommand => new CommandBuilder(_container, this);

        public RoomConfiguration Rooms { get; }

        public GameConfiguration(IDependencyContainer container, Game game)
        {
            _container = container;
            _game = game;
            Rooms = new RoomConfiguration(this, game.Content);
        }
        
        internal void Validate()
        {
            foreach (var entity in Entities)
                entity.Validate();
            Rooms.Validate();
        }

        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    }
}