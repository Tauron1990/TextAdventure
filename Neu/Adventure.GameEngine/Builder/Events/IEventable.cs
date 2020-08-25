using Adventure.GameEngine.Builder.Core;

namespace Adventure.GameEngine.Builder.Events
{
    public interface IEventable<out TEventSource, out TEventData>
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        TEventSource EventSource { get; }

        TEventData EventData { get; }
    }
}