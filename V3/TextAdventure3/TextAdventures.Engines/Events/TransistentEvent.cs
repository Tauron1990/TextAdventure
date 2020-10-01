using System;
using Akka.Event;
using Akkatecture.Aggregates;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Events
{
    public abstract class TransistentEvent<TEvent> : AggregateEvent<GameInfo, GameInfoId>
        where TEvent : TransistentEvent<TEvent>
    {
        public virtual void Publish(EventStream stream)
        {
            var eventMetadata = new Metadata
                                {
                                    Timestamp = DateTimeOffset.Now,
                                    AggregateSequenceNumber = 1,
                                    AggregateName = nameof(GameInfo),
                                    AggregateId = GameInfoId.Id.Value,
                                    EventName = typeof(TEvent).Name,
                                    EventVersion = 1
                                };
            var domainfEvent = new DomainEvent<GameInfo, GameInfoId, TEvent>(GameInfoId.Id, (TEvent)this, eventMetadata, DateTimeOffset.Now, 1);
            stream.Publish(domainfEvent);
        }
    }
}