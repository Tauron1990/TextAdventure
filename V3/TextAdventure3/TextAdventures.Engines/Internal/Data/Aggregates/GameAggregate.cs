using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Core;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public abstract class GameState<TState, TAggregate, TIdentity> : AggregateState<TAggregate, TIdentity>, IHydrate<TState>, IAggregateSnapshot<TAggregate, TIdentity>
        where TState : IAggregateSnapshot 
        where TIdentity : IIdentity 
        where TAggregate : IAggregateRoot<TIdentity>
    {
        public abstract void Hydrate(TState aggregateSnapshot);
    }

    public abstract class GameAggregate<TAggregate, TIdentity, TAggregateState> : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : AggregateState<TAggregate, TIdentity, IMessageApplier<TAggregate, TIdentity>>, IAggregateSnapshot<TAggregate, TIdentity>
        where TIdentity : IIdentity
    {
        protected GameAggregate(TIdentity id) 
            : base(id)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            SetSnapshotStrategy(new SnapshotEveryFewVersionsStrategy(100));
        }

        protected override IAggregateSnapshot<TAggregate, TIdentity>? CreateSnapshot()
        {
            return State;
        }
    }
}