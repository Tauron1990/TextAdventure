using System.Collections.Generic;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Internal
{
    public sealed class EntityData
    {
        [JsonConverter(typeof(TypeAwareConverter))]
        public List<IComponent> Components { get; }

        [JsonConstructor]
        public EntityData(List<IComponent> components) 
            => Components = components;

        public EntityData() => Components = new List<IComponent>();
    }
}