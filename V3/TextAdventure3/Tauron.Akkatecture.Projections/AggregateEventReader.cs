using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Util;
using Akkatecture.Aggregates;
using Akkatecture.Query;
using JetBrains.Annotations;
using LiquidProjections;
using LiquidProjections.Abstractions;

namespace Tauron.Akkatecture.Projections
{
    [PublicAPI]
    public sealed class EventReaderException : EventArgs
    {
        public Type Aggregate { get; }

        public Exception Exception { get; }

        public EventReaderException(Type aggregate, Exception exception)
        {
            Aggregate = aggregate;
            Exception = exception;
        }
    }

    [PublicAPI]
    public abstract class AggregateEventReader
    {
        public event EventHandler<EventReaderException>? OnReadError; 

        public abstract IDisposable CreateSubscription(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId);

        protected virtual void OnOnReadError(EventReaderException e) => OnReadError?.Invoke(this, e);
    }

    [PublicAPI]
    public sealed class AggregateEventReader<TJournal> : AggregateEventReader 
        where TJournal : IEventsByTagQuery, ICurrentEventsByTagQuery
    {
        private readonly ActorSystem _system;
        private readonly string _journalId;
        private readonly ActorMaterializer _actorMaterializer;

        public AggregateEventReader(ActorSystem system, string journalId)
        {
            _system = system;
            _journalId = journalId;
            _actorMaterializer = system.Materializer();
        }

        public override IDisposable CreateSubscription(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId)
        {
            var genericType = typeof(SubscriptionBuilder<>).MakeGenericType(Type.GetType(subscriptionId));
            var subscriberInst = (SubscriptionBuilder)genericType.FastCreateInstance(_actorMaterializer, _system, _journalId, subscriber)!;

            return subscriberInst.CreateSubscription(lastProcessedCheckpoint, (type, exception) => OnOnReadError(new EventReaderException(type, exception)));
        }

        private abstract class SubscriptionBuilder : IDisposable
        {
            public abstract void Dispose();
            public abstract IDisposable CreateSubscription(long? lastProcessedCheckpoint, Action<Type, Exception> errorHandler);
        }

        private sealed class SubscriptionBuilder<TAggregate> : SubscriptionBuilder
            where TAggregate : IAggregateRoot
        {
            private readonly AtomicBoolean _isCancel = new AtomicBoolean();

            private readonly ActorMaterializer _materializer;
            private readonly ActorSystem _system;
            private readonly string _journalId;
            private readonly Subscriber _subscriber;

            private UniqueKillSwitch? _cancelable;
            private Task? _runner;
            private Action<Type, Exception>? _errorHandler;

            public SubscriptionBuilder(ActorMaterializer materializer, ActorSystem system, string journalId, Subscriber subscriber)
            {
                _materializer = materializer;
                _system = system;
                _journalId = journalId;
                _subscriber = subscriber;
            }

            public override IDisposable CreateSubscription(long? lastProcessedCheckpoint, Action<Type, Exception> errorHandler)
            {
                _errorHandler = errorHandler;
                lastProcessedCheckpoint ??= 0;
                var consumer = Consumer.Create(_system).Using<TJournal>(_journalId);

                var source = consumer.EventsFromAggregate<TAggregate>(Offset.Sequence(lastProcessedCheckpoint.Value))
                   .Select(ee => ee.Event as IDomainEvent)
                   .Where(de => de != null)
                   .Batch(20, de => ImmutableList<IDomainEvent>.Empty.Add(de!), (list, evt) => list.Add(evt!))
                   .Select(de =>
                    {
                        var last = de.Last();

                        return new Transaction
                               {
                                   Checkpoint = last.AggregateSequenceNumber,
                                   Id = EventId.New.Value,
                                   StreamId = last.GetIdentity().Value,
                                   TimeStampUtc = last.Timestamp.DateTime,
                                   Events = new List<LiquidProjections.EventEnvelope>(
                                       de
                                          .Select(evt =>
                                           {
                                               return new LiquidProjections.EventEnvelope
                                                      {
                                                          Body = evt,
                                                          Headers = evt
                                                             .Metadata
                                                             .Select(p => Tuple.Create<string, object>(p.Key, p.Value))
                                                             .ToDictionary(t => t.Item1, t => t.Item2)
                                                      };
                                           }))
                               };
                    })
                   .Batch(5, t => ImmutableList<Transaction>.Empty.Add(t), (list, transaction) => list.Add(transaction))
                   .AlsoTo(Sink.OnComplete<ImmutableList<Transaction>>(
                        () => _isCancel.GetAndSet(true),
                        e =>
                        {
                            _isCancel.GetAndSet(true);
                            errorHandler(typeof(TAggregate), e);
                        }))
                   .ViaMaterialized(KillSwitches.Single<ImmutableList<Transaction>>(), (_, kill) => kill)
                   .PreMaterialize(_materializer);

                _cancelable = source.Item1;

                var sinkQueue = source.Item2.RunWith(Sink.Queue<ImmutableList<Transaction>>()
                   .WithAttributes(new Attributes(new Attributes.InputBuffer(2, 2))), _materializer);

                _runner = Run(sinkQueue);

                return this;
            }

            private async Task Run(ISinkQueue<ImmutableList<Transaction>> queue)
            {

                while (!_isCancel.Value)
                {
                    try
                    {
                        var data = await queue.PullAsync();

                        if (data.HasValue)
                        {
                            await _subscriber.HandleTransactions(data.Value, new SubscriptionInfo
                                                                             {
                                                                                 Id = data.Value.Last().StreamId, 
                                                                                 Subscription = this
                                                                             });
                        }
                        else
                            Thread.Sleep(1);
                    }
                    catch (Exception e)
                    {
                        _errorHandler?.Invoke(typeof(TAggregate), e);
                    }
                }
            }

            public override void Dispose()
            {
                _isCancel.GetAndSet(true);
                _cancelable?.Shutdown();
                _runner?.Wait(TimeSpan.FromSeconds(20));
            }
        }
    }
}