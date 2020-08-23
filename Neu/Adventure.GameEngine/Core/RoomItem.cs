using Adventure.GameEngine.Systems.Components;
using EcsRx.Entities;

namespace Adventure.GameEngine.Core
{
    public sealed class RoomItem
    {
        public IEntity Entity { get; }

        public IngameObject Data { get; }

        public RoomItem(IEntity entity, IngameObject data)
        {
            Entity = entity;
            Data = data;
        }
    }
}