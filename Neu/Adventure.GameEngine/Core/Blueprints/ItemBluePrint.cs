using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class ItemBluePrint : IBlueprint
    {
        public LazyString Description { get; set; } = LazyString.New();

        public string Action { get; set; } = string.Empty;

        public string? TriggerEvent { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Id { get; }

        public string DisplayName { get; set; } = string.Empty;

        public bool CanDrop { get; set; }

        public ItemBluePrint(string id) => Id = id;

        public void Apply(IEntity entity)
        {
            entity.AddComponents(new IngameObject(Id, Description, Location, DisplayName));
            if (CanDrop)
                entity.AddComponents(new InteractiveObject(Action, TriggerEvent));
        }
    }
}