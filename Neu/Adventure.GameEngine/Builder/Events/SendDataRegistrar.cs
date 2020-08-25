using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Builder.Events
{
    public class SendDataRegistrar<TSource, TEventData> : EventRegistrar<TSource, TEventData> 
        where TSource : IWithMetadata, IEntityConfiguration
    {
        protected internal override IBlueprint CreateRegistration(TSource eventSource, TEventData data, string name) =>
            new EntityEvent<TEventData>(data, name);
    }
}