using Adventure.GameEngine.Components;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class PlayerSetup : IBlueprint
    {
        private readonly string _playerLocation;

        public PlayerSetup(string playerLocation) => _playerLocation = playerLocation;

        public void Apply(IEntity entity)
        {
            entity.AddComponents(new IComponent[]{ new Actor(_playerLocation), new IsPlayerControlled(), new HasInvertory() });
        }
    }
}