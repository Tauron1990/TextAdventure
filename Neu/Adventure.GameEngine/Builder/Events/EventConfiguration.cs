using Adventure.GameEngine.Builder.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.Events
{
    [PublicAPI]
    public sealed class EventConfiguration<TEventSource, TData>
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        public TEventSource EventSource { get; }

        public TData Data { get; }

        public EventRegistrar<TEventSource, TData>? Registrar { get; set; }

        public EventConfiguration(TEventSource eventSource, TData data)
        {
            EventSource = eventSource;
            Data = data;
        }

        internal void Register(IEventable<TEventSource, TData> source, string name)
        {
            if(Registrar == null) return;

            source.EventSource.AddBlueprint(Registrar.CreateRegistration(EventSource, Data, name));
        }
    }
}