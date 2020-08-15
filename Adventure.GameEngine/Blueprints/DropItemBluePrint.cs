using Adventure.GameEngine.Components;
using Adventure.GameEngine.Core;
using Adventure.TextProcessing.Synonyms;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Blueprints
{
    public sealed class DropItemBluePrint : IBlueprint
    {
        public LazyString Description { get; set; } = new LazyString(string.Empty);

        public VerbCodes Action { get; set; } = VerbCodes.Custom;

        public string? TriggerEvent { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Id { get; }

        public DropItemBluePrint(string id) => Id = id;

        public void Apply(IEntity entity)
        {
            entity.AddComponents(new IngameObject(Id, Description, new ReactiveProperty<string>(Location)), new InteractiveObject(Action, TriggerEvent));
        }
    }
}