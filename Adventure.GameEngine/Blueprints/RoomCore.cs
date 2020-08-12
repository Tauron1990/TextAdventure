using Adventure.GameEngine.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class RoomCore : IBlueprint
    {
        private readonly string _name;

        public RoomCore(string name) => _name = name;

        public void Apply(IEntity entity) 
            => entity.AddComponent(new Room(_name), new RoomData());
    }
}