using Adventure.GameEngine.Core;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Components
{
    public sealed class IngameObject : IComponent
    {
        public string Id { get; }

        public ReactiveProperty<string> Location { get; }

        public LazyString Description { get; set; }

        public string DisplayName { get; }

        public IngameObject(string id, LazyString description, ReactiveProperty<string> location, string displayName)
        {
            Id = id;
            Description = description;
            Location = location;
            DisplayName = displayName;
        }
    }
}