using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class RoomEvent<TData> : IBlueprint
    {
        private readonly TData _data;

        private readonly string _name;

        public RoomEvent(TData data, string name)
        {
            _data = data;
            _name = name;
        }

        public void Apply(IEntity entity)
            => entity.GetComponent<RoomEvents>().AddEvent(_name, _data);
    }
}