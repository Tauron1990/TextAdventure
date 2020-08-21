using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class StartRoom : IBlueprint
    {
        public void Apply(IEntity entity)
        {
            var comp = entity.GetComponent<RoomData>();

            comp.IsPlayerIn.Value = true;
            comp.IsVisited.Value = true;
        }
    }
}