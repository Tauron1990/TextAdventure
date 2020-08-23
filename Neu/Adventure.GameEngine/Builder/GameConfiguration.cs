using System.Collections.Generic;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.ItemData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Pools;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class GameConfiguration : IInternalGameConfiguration
    {
        private readonly IInternalGameConfiguration _internalGameConfigurationImplementation = new InternalConfiguration();

        private readonly IDependencyContainer _container;
        private readonly Game _game;

        internal List<IBluePrintProvider> Entities { get; } = new List<IBluePrintProvider>();

        public CommandBuilder NewCommand => new CommandBuilder(_container, this, this);

        public RoomConfiguration Rooms { get; }

        public GameConfiguration(IDependencyContainer container, Game game)
        {
            _container = container;
            _game = game;
            Rooms = new RoomConfiguration(this, game.Content);
        }

        CommandId IInternalGameConfiguration.RegisterCommand(Command command) =>
            _internalGameConfigurationImplementation.RegisterCommand(command);

        Command IInternalGameConfiguration.GetCommand(CommandId id) =>
            _internalGameConfigurationImplementation.GetCommand(id);

        ItemId IInternalGameConfiguration.RegisterItem(ItemBuilder item)
            => _internalGameConfigurationImplementation.RegisterItem(item);

        internal void Validate()
        {
            foreach (var entity in Entities)
                entity.Validate();
            Rooms.Validate();
        }

        private sealed class InternalConfiguration : IInternalGameConfiguration
        {
            private readonly IIdPool _ids = new IdPool(5, 50);

            private readonly Dictionary<CommandId, Command> _commands = new Dictionary<CommandId, Command>();

            private readonly Dictionary<ItemId, ItemBuilder> _items = new Dictionary<ItemId, ItemBuilder>();

            public CommandId RegisterCommand(Command command)
            {
                var id = new CommandId(_ids.AllocateInstance());
                _commands[id] = command;
                return id;
            }

            public Command GetCommand(CommandId id)
                => _commands[id];

            public ItemId RegisterItem(ItemBuilder item)
            {
                var id = new ItemId(item.BluePrint.Id);
                _items.Add(id, item);
                return id;
            }
        }
    }
}