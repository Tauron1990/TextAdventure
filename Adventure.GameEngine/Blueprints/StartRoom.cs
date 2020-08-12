using Adventure.GameEngine.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class StartRoom : IBlueprint
    {
        public void Apply(IEntity entity) 
            => entity.GetComponent<RoomData>().IsPlayerIn.Value = true;
    }
}