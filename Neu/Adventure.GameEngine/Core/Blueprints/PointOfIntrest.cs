using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class PointOfIntrestAddr : IBlueprint
    {
        private readonly PointOfInterst? _intrest;

        public PointOfIntrestAddr(PointOfInterst? intrest)
            => _intrest = intrest;

        public void Apply(IEntity entity)
        {
            if(_intrest == null) return;

            entity.GetComponent<RoomData>().Pois.Add(_intrest);
        }
    }
}