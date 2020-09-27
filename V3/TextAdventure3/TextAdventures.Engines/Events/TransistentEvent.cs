using System;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace TextAdventures.Engine.Events
{
    public abstract class TransistentEvent<TEvent> : IDomainEvent, IAggregateEvent, IIdentity
        where TEvent : TransistentEvent<TEvent>
    {
        public virtual Type AggregateType => typeof(TransistentEvent<TEvent>);

        public virtual Type IdentityType => typeof(TransistentEvent<TEvent>);

        public virtual Type EventType => GetType();

        public virtual long AggregateSequenceNumber => 0;

        public virtual Metadata Metadata => new Metadata();

        public virtual DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

        public virtual IIdentity GetIdentity() => this;

        public virtual IAggregateEvent GetAggregateEvent() => throw new NotImplementedException();

        string IIdentity.Value => string.Empty;
    }
}