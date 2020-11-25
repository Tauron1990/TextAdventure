using System;
using System.Linq.Expressions;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace TextAdventures.Engine.Processors
{
    public abstract class GameSagaId<T> : SagaId<T>
        where T : SagaId<T>
    {
        protected GameSagaId(string value)
            : base(value) { }
    }

    public abstract class GameSagaLocator<TIdentity> : ISagaLocator<TIdentity>
        where TIdentity : GameSagaId<TIdentity>
    {
        public abstract TIdentity LocateSaga(IDomainEvent domainEvent);
    }

    public abstract class GameSageState<TState, TSaga, TIdentity> : SagaState<TSaga, TIdentity, IMessageApplier<TSaga, TIdentity>>, IAggregateSnapshot<TSaga, TIdentity>, IHydrate<TState>
        where TState : GameSageState<TState, TSaga, TIdentity>
        where TIdentity : GameSagaId<TIdentity>
        where TSaga : GameSaga<TSaga, TIdentity, TState>
    {
        public abstract void Hydrate(TState aggregateSnapshot);
    }

    public abstract class GameSaga<TAggregateSaga, TIdentity, TSagaState> : AggregateSaga<TAggregateSaga, TIdentity, TSagaState>
        where TAggregateSaga : GameSaga<TAggregateSaga, TIdentity, TSagaState>
        where TIdentity : GameSagaId<TIdentity>
        where TSagaState : GameSageState<TSagaState, TAggregateSaga, TIdentity>
    {
        protected GameSaga()
        {
            SetSnapshotStrategy(new SnapshotEveryFewVersionsStrategy(100));
        }

        protected override IAggregateSnapshot<TAggregateSaga, TIdentity>? CreateSnapshot() => State;
    }

    public abstract class GameSagaManger<TAggregateSaga, TIdentity, TSagaLocator> : AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TIdentity : GameSagaId<TIdentity>
        where TSagaLocator : GameSagaLocator<TIdentity>, new()
    {
        protected GameSagaManger(Expression<Func<TAggregateSaga>> sagaFactory) : base(sagaFactory) { }
    }
}