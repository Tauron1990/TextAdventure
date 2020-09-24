using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akkatecture.Aggregates;
using LiquidProjections;
using LiquidProjections.Abstractions;

namespace TextAdventures.Engine.Internal.Data.Projection
{
    public sealed class EventStoreReader : IDisposable
    {
        private readonly ActorSystem _system;
        private readonly List<Subscription> _subscriptions = new List<Subscription>();

        public EventStoreReader(ActorSystem system) 
            => _system = system;

        public IDisposable CreateSubscription(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId)
        {
            var sub = new Subscription(subscriptionId, subscriber, lastProcessedCheckpoint ?? 0,
                PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier), _system, subscription =>
                {
                    lock (_subscriptions)
                        _subscriptions.Remove(subscription);
                });

            lock(_subscriptions)
                _subscriptions.Add(sub);

            sub.Start();

            return sub;
        }

        private class Subscription : IDisposable
        {
            private readonly string _id;
            private readonly Subscriber _subscriber;
            private readonly long _point;
            private readonly SqlReadJournal _journal;
            private readonly Action<Subscription> _dispose;
            private readonly ActorMaterializer _materializer;

            private IKillSwitch? _switch;

            public Subscription(string id, Subscriber subscriber, long point, SqlReadJournal journal, ActorSystem system, Action<Subscription> dispose)
            {
                _id = id;
                _subscriber = subscriber;
                _point = point;
                _journal = journal;
                _dispose = dispose;

                _materializer = system.Materializer();
            }

            public async void Start()
            {
                var _isRunning = false;

                try
                {
                    var sink = Sink.ForEach<ImmutableList<Transaction>>(async e =>
                        await _subscriber.HandleTransactions(e, new SubscriptionInfo
                                                                {
                                                                    Id = _id,
                                                                    Subscription = this
                                                                }));

                    var (killSwitch, task) = _journal.EventsByTag(_id, new Sequence(_point))
                       .Select(e => new Transaction
                                    {
                                        Checkpoint = e.SequenceNr,
                                        Id = Guid.NewGuid().ToString(),
                                        StreamId = e.PersistenceId,
                                        TimeStampUtc = ((IAggregateEvent) e.Event).Timestamp,
                                        Events = new List<LiquidProjections.EventEnvelope>
                                                 {
                                                     new LiquidProjections.EventEnvelope
                                                     {
                                                         Body = e.Event
                                                     }
                                                 }
                                    })
                       .Batch(50, e => ImmutableList<Transaction>.Empty.Add(e), (array, envelope) => array.Add(envelope))
                       .ViaMaterialized(KillSwitches.Single<ImmutableList<Transaction>>(), Keep.Right)
                       .ToMaterialized(sink, Keep.Both)
                       .Run(_materializer);

                    _switch = killSwitch;

                    await task;
                }
                catch
                {
                    if (!_isRunning)
                    {
                        await _subscriber.NoSuchCheckpoint(new SubscriptionInfo
                                                           {
                                                               Id = _id,
                                                               Subscription = this
                                                           });

                    }

                    _materializer.Dispose();
                }
            }

            public void Dispose()
            {
                _switch?.Shutdown();
                _materializer.Dispose();
                _dispose(this);
            }
        }

        public void Dispose()
        {
            lock (_subscriptions)
            {
                foreach (var subscription in _subscriptions) subscription.Dispose();
            }
        }
    }
}