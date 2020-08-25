using Adventure.GameEngine.Builder.Core;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Builder.Events
{
    public abstract class EventRegistrar<TEventSource, TData>
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        internal protected abstract IBlueprint CreateRegistration(TEventSource eventSource, TData data, string name);
    }
}