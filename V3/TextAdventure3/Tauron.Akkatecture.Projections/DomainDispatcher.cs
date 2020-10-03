using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LiquidProjections;
using LiquidProjections.Abstractions;

namespace Tauron.Akkatecture.Projections
{
    [PublicAPI]
    public class DomainDispatcher<TAggregate, TProjection, TIdentity>
    {
        public AggregateEventReader Reader { get; }
        public DomainProjector Projector { get; }
        public IProjectionRepository Repo { get; }

        protected internal readonly Dispatcher Dispatcher;

        public DomainDispatcher(AggregateEventReader reader, DomainProjector projector, IProjectionRepository repo)
        {
            Reader = reader;
            Projector = projector;
            Repo = repo;

            Dispatcher = new Dispatcher(reader.CreateSubscription)
                         {
                             ExceptionHandler = ExceptionHandler,
                             SuccessHandler = SuccessHandler
                         };
        }

        protected virtual Task SuccessHandler(SubscriptionInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<ExceptionResolution> ExceptionHandler(Exception exception, int attempts, SubscriptionInfo info) 
            => !exception.IsCriticalApplicationException() && attempts < 3 ? Task.FromResult(ExceptionResolution.Retry) : Task.FromResult(ExceptionResolution.Abort);

        public IDisposable Subscribe()
        {
            var options = new SubscriptionOptions {Id = typeof(TAggregate).AssemblyQualifiedName};

            return Dispatcher.Subscribe(Repo.GetLastCheckpoint<TProjection, TIdentity>())
        }
    }
}