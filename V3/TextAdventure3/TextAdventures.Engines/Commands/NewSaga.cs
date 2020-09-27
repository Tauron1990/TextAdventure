using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using TextAdventures.Builder.Commands;

namespace TextAdventures.Engine.Commands
{
    public sealed class NewSaga<TSaga, TAggregateSaga, TIdentity, TSagaLocator> : INewSaga
        where TSaga : AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>, new()
    {
        private object[] _params = Array.Empty<object>();

        public Props SagaManager => Props.Create<TSaga>(_params);

        public string Name => typeof(TSaga).Name;

        public NewSaga<TSaga, TAggregateSaga, TIdentity, TSagaLocator> With(params object[] parameters)
        {
            _params = parameters;
            return this;
        }
    }

    public static class NewSaga
    {
        public static NewSaga<TSaga, TAggregateSaga, TIdentity, TSagaLocator> Create<TSaga, TAggregateSaga, TIdentity, TSagaLocator>()
            where TSaga : AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
            where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
            where TIdentity : SagaId<TIdentity>
            where TSagaLocator : class, ISagaLocator<TIdentity>, new()
            => new NewSaga<TSaga, TAggregateSaga, TIdentity, TSagaLocator>();
    }
}