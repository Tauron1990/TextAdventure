using System;
using Adventure.GameEngine.Components;
using Adventure.TextProcessing.Interfaces;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class RoomCommandSetup : IBlueprint
    {
        private readonly Func<ICommand, string?> _handler;

        public RoomCommandSetup(Func<ICommand, string?> handler) => _handler = handler;
        public void Apply(IEntity entity)
        {
            if (entity.HasComponent(typeof(RoomCommands)))
                entity.GetComponent<RoomCommands>().Add(_handler);
            else
                entity.AddComponent(new RoomCommands(_handler));
        }
    }
}