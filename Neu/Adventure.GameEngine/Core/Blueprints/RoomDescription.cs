using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class RoomDescription : IBlueprint
    {
        private readonly string _description;

        public RoomDescription(string description) => _description = description;

        public void Apply(IEntity entity) 
            => entity.GetComponent<RoomData>().Description.Value = _description;
    }
}