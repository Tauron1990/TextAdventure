using System.Collections.Generic;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
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

        public CommandBuilder NewCommand => new CommandBuilder(_container, this);

        public RoomConfiguration Rooms { get; }

        public GameConfiguration(IDependencyContainer container)
        {
            _container = container;
            Rooms = new RoomConfiguration(this);
        }

        CommandId IInternalGameConfiguration.RegisterCommand(Command command) =>
            _internalGameConfigurationImplementation.RegisterCommand(command);

        Command IInternalGameConfiguration.GetCommand(CommandId id) =>
            _internalGameConfigurationImplementation.GetCommand(id);

        private sealed class InternalConfiguration : IInternalGameConfiguration
        {
            private readonly IIdPool _ids = new IdPool(5, 50);

            private readonly Dictionary<CommandId, Command> _commands = new Dictionary<CommandId, Command>();

            public CommandId RegisterCommand(Command command)
            {
                var id = new CommandId(_ids.AllocateInstance());
                _commands[id] = command;
                return id;
            }

            public Command GetCommand(CommandId id)
                => _commands[id];
        }

        internal void Validate() =>
            Rooms.Validate();
    }
}