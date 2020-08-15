using Adventure.GameEngine.Core;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Components
{
    public sealed class IngameObject : IComponent
    {
        public string Id { get; }

        public ReactiveProperty<string> Locarion { get; }

        public LazyString Description { get; set; }

        public IngameObject(string id, LazyString description, ReactiveProperty<string> locarion)
        {
            Id = id;
            Description = description;
            Locarion = locarion;
        }
    }
}