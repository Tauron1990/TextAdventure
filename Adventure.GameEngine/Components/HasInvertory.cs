using System.Collections.Immutable;
using EcsRx.Components;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    [PublicAPI]
    public sealed class HasInvertory : IComponent
    {
        public ImmutableDictionary<string, int> Items { get; private set; }

        [JsonConstructor]
        public HasInvertory(ImmutableDictionary<string, int> items) => Items = items;


        public HasInvertory() => Items = ImmutableDictionary<string, int>.Empty;


        public void Add(string item) 
            => Items = Items.ContainsKey(item) ? Items.SetItem(item, Items[item] + 1) : Items.SetItem(item, 1);

        public void Remove(string item)
        {
            if (!Items.TryGetValue(item, out var amount)) return;
            
            Items = amount == 1 ? Items.Remove(item) : Items.SetItem(item, amount - 1);
        }
    }
}