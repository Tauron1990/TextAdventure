using System.Collections.Immutable;

namespace TextAdventures.Engine.Data
{
    public abstract class ComponentBase
    {
        private ImmutableDictionary<string, string> ComponentData { get; set; } = ImmutableDictionary<string, string>.Empty;
        private ImmutableDictionary<string, object> _cache = ImmutableDictionary<string, object>.Empty;

        public TType GetData<TType>()
    }
}