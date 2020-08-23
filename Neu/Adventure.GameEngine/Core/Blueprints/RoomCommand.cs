using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class RoomCommand : IBlueprint
    {
        private readonly Command _command;

        private readonly LazyString _commandName;

        public RoomCommand(Command command, LazyString commandName)
        {
            _command = command;
            _commandName = commandName;
        }

        public void Apply(IEntity entity)
        {
            var commands = entity.HasComponent<RoomCommands>() ? entity.GetComponent<RoomCommands>() : entity.AddComponent<RoomCommands>();

            commands.Add(_commandName, _command);
        }
    }
}