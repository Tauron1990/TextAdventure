using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class RoomCore : IBlueprint
    {
        private readonly string _name;

        public RoomCore(string name) => _name = name;

        public void Apply(IEntity entity) 
            => entity.AddComponents(new IComponent[] { new Room(_name), new RoomData() });
    }
}