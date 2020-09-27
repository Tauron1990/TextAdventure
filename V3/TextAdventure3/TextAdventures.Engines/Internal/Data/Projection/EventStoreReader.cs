using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akkatecture.Aggregates;
using LiquidProjections;
using LiquidProjections.Abstractions;
using EventEnvelope = Akka.Persistence.Query.EventEnvelope;

namespace TextAdventures.Engine.Internal.Data.Projection
{
    public sealed class EventStoreReader : IDisposable
    {
        private readonly ActorSystem _system;
        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        private readonly ConcurrentDictionary<string, Task<IDisposable>> _waiter = new ConcurrentDictionary<string, Task<IDisposable>>();
        private readonly ActorMaterializer _materializer;

        public EventStoreReader(ActorSystem system)
        {
            _system = system;
            _materializer = system.Materializer(namePrefix: "EventStoreMaterializer");
        }

        public void AddLoadingSequence(string tag, Task<IDisposable> waiter) 
            => _waiter[tag] = waiter;

        public IDisposable CreateSubscription(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId)
        {
            var sub = new Subscription(subscriptionId, subscriber, lastProcessedCheckpoint ?? 0,
                PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier), _materializer, subscription =>
                {
                    lock (_subscriptions)
                        _subscriptions.Remove(subscription);
                }, s =>
                {
                    if (_waiter.TryRemove(s, out var task))
                        task.ContinueWith(t => t.Result.Dispose());
                });

            lock(_subscriptions)
                _subscriptions.Add(sub);

            Task.Run(async () =>  await sub.Start());

            return sub;
        }

        public void Dispose()
        {
            lock (_subscriptions)
            {
                foreach (var subscription in _subscriptions)
                    subscription.Dispose();
            }
            _materializer.Dispose();
        }

        private class Subscription : IDisposable
        {
            private static readonly Flow<EventEnvelope, ImmutableList<Transaction>, UniqueKillSwitch> Transformator = 
                Flow.Create<EventEnvelope>()
               .Select(e =>
                    {
                        var commit = (ICommittedEvent) e.Event;

                        return new Transaction
                               {
                                   Checkpoint = e.SequenceNr,
                                   Id = Guid.NewGuid().ToString(),
                                   StreamId = e.PersistenceId,
                                   TimeStampUtc = commit.Timestamp.UtcDateTime,
                                   Events = new List<LiquidProjections.EventEnvelope>
                                            {
                                                new LiquidProjections.EventEnvelope
                                                {
                                                    Body = commit.GetAggregateEvent(),
                                                    Headers = commit.Metadata.ToDictionary(p => p.Key, p => (object)p.Value)
                                                }
                                            }
                               };
                    })
               .Batch(50, e => ImmutableList<Transaction>.Empty.Add(e), (array, envelope) => array.Add(envelope))
               .ViaMaterialized(KillSwitches.Single<ImmutableList<Transaction>>(), Keep.Right);

            private readonly string _tag;
            private readonly Subscriber _subscriber;
            private readonly long _point;
            private readonly SqlReadJournal _journal;
            private readonly Action<Subscription> _dispose;
            private readonly Action<string> _loadingCompled;
            private readonly IMaterializer _materializer;

            private IKillSwitch? _switch;



            public Subscription(string tag, Subscriber subscriber, long point, SqlReadJournal journal, IMaterializer system, Action<Subscription> dispose, Action<string> loadingCompled)
            {
                _tag = tag;
                _subscriber = subscriber;
                _point = point;
                _journal = journal;
                _dispose = dispose;
                _loadingCompled = loadingCompled;
                _materializer = system;
            }

            public async Task Start()
            {
                var isRunning = false;

                try
                {
                    var projectorProcessor = Flow.Create<ImmutableList<Transaction>>()
                       .Select(async t =>
                        {
                            await _subscriber.HandleTransactions(t, new SubscriptionInfo
                                                                    {
                                                                        Id = _tag,
                                                                        Subscription = this
                                                                    });

                            return t[^1];
                        });

                    var offsetFinder = Sink.LastOrDefault<Transaction>();

                    var loader = _journal
                       .CurrentEventsByTag(_tag)
                       .ViaMaterialized(Transformator.Via(projectorProcessor), (_, kill) => kill)
                       .Select(t => t.Result)
                       .ToMaterialized(offsetFinder, (killSwitch, task) => (killSwitch, task))
                       .Run(_materializer);

                    isRunning = true;
                    Interlocked.Exchange(ref _switch, loader.killSwitch);

                    var result = await loader.task;
                    _loadingCompled(_tag);

                    var runner = _journal
                       .EventsByTag(_tag, Offset.Sequence(result?.Checkpoint ?? 0))
                       .ViaMaterialized(Transformator.Via(projectorProcessor), (_, kill) => kill)
                       .Select(t => t.Result)
                       .ToMaterialized(Sink.Ignore<Transaction>(), (kill, task) => (kill, task))
                       .Run(_materializer);

                    Interlocked.Exchange(ref _switch, runner.kill)?.Shutdown();
                    await runner.task;
                }
                catch(Exception e)
                {
                    _switch?.Abort(e);
                    _loadingCompled(_tag);
                    if (!isRunning)
                    {
                        await _subscriber.NoSuchCheckpoint(new SubscriptionInfo
                                                           {
                                                               Id = _tag,
                                                               Subscription = this
                                                           });

                    }
                }
            }

            public void Dispose()
            {
                _switch?.Shutdown();
                _dispose(this);
            }
        }
    }
}