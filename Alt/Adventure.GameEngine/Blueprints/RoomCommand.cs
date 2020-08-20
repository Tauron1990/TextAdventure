using System.Collections.Generic;
using Adventure.GameEngine.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class RoomCommandSetup : IBlueprint
    {
        private readonly CommandHandler _handler;
        private readonly IEnumerable<string> _processedCommands;

        public RoomCommandSetup(CommandHandler handler, IEnumerable<string> processedCommands)
        {
            _handler = handler;
            _processedCommands = processedCommands;
        }

        public void Apply(IEntity entity)
        {
            if (!entity.HasComponent(typeof(RoomCommands)))
                entity.AddComponents(new RoomCommands());

            entity.GetComponent<RoomCommands>().Add(_handler, _processedCommands);
        }
    }
}