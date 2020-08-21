using System.Collections.Generic;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using EcsRx.Pools;

namespace Adventure.GameEngine.Builder
{
    public sealed class GameConfiguration
    {
        public CommandBuilder NewCommand { get; }

        private sealed class InternalComandiguration : IInternalGameConfiguration
        {
            private readonly IIdPool _ids = new IdPool(5, 50);

            private readonly Dictionary<CommandId, Command> _commands = new Dictionary<CommandId, Command>();

            public CommandId RegisterCommand(Command command)
                => throw new System.NotImplementedException();

            public Command GetCommand(CommandId id)
                => throw new System.NotImplementedException();
        }
    }
}