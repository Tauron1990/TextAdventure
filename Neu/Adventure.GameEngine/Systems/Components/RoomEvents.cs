using System.Collections.Immutable;
using EcsRx.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class RoomEvents : IComponent
    {
        public ImmutableDictionary<string, EventData> Events { get; private set; } = ImmutableDictionary<string, EventData>.Empty;

        public void AddEvent<TData>(string name, TData data)
            => Events = Events.Add(name, new EventData<TData>(data));
    }
}