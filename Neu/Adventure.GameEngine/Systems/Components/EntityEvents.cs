using System.Collections.Immutable;
using EcsRx.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class EntityEvents : IComponent
    {
        public ImmutableDictionary<string, ImmutableList<EventData>> Events { get; private set; } = ImmutableDictionary<string, ImmutableList<EventData>>.Empty;

        public void AddEvent<TData>(string name, TData data)
        {
            var evt = new EventData<TData>(data);
            ImmutableList<EventData> list = Events.ContainsKey(name) ? Events[name].Add(evt) : ImmutableList<EventData>.Empty.Add(evt);

            Events = Events.SetItem(name, list);
        }
    }
}