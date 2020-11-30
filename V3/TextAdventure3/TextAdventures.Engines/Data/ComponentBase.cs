using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TextAdventures.Builder;

namespace TextAdventures.Engine.Data
{
    [PublicAPI]
    public abstract class ComponentBase : IComponent
    {
        internal ImmutableDictionary<string, object?> Cache = ImmutableDictionary<string, object?>.Empty;
        internal ImmutableDictionary<string, object> DefaultValues = ImmutableDictionary<string, object>.Empty;

        public TType GetData<TType>(TType defaultValue, [CallerMemberName] string? name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if ((Cache.TryGetValue(name, out var data) || DefaultValues.TryGetValue(name, out data)) && data is TType casted)
                return casted;

            return defaultValue;
        }

        public void SetData<TType>(TType value, [CallerMemberName] string? name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Cache = Equals(value, null) ? Cache.SetItem(name, null) : Cache.SetItem(name, value!);
        }

        protected internal virtual void ApplyEvent(object @event) { }
    }
}