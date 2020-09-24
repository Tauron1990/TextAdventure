using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akkatecture.Core;
using LiquidProjections;
using Tauron;
using Tauron.Akka;
using TextAdventures.Engine.Internal.Data.Events;
using TextAdventures.Engine.Internal.Data.Projection;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class ProjectionManager : ExposedReceiveActor
    {
        private readonly GameProjection _gameProjection = new GameProjection();
        private readonly EventStoreReader _eventStoreReader = new EventStoreReader(Context.System);
        private readonly List<IDisposable> _disposable = new List<IDisposable>();
        private readonly Dictionary<Type, IQueryHandler> _queryHandlers = new Dictionary<Type, IQueryHandler>();

        public ProjectionManager()
        {
            Receive<IGameQuery>(q =>
            {
                var queryType = q.GetType();
                foreach (var queryHandler in _queryHandlers.Where(queryHandler => !queryHandler.Key.IsAssignableFrom(queryType)))
                {
                    queryHandler.Value.Handle(q, Sender);
                    return true;
                }

                return false;
            });
            Receive<StartGame>(ConstructProjection);
        }

        protected override void PostStop()
        {
            foreach (var disposable in _disposable) 
                disposable.Dispose();
            _eventStoreReader.Dispose();
            base.PostStop();
        }

        private void ConstructProjection(StartGame obj)
        {
            _disposable.Add(
                ConstructProjector("Room", _gameProjection.Rooms, 
                    eventBuilder =>
                    {
                        eventBuilder
                           .Map<RoomCreatedEvent>()
                           .AsCreateOf(e => e.Id)
                           .IgnoringDuplicates()
                           .Using((projection, createdEvent) => projection.Id = createdEvent.Id);
                    }));

        }

        private IDisposable ConstructProjector<TKey, TProjector>(string tag, ConcurrentDictionary<TKey, TProjector> store, Action<EventMapBuilder<TProjector, TKey, ProjectionContext>> mapEvents)
            where TKey : IIdentity
            where TProjector : class, IProjectorData<TKey>, new()
        {
            var eventBuilder = new EventMapBuilder<TProjector, TKey, ProjectionContext>();

            mapEvents(eventBuilder);

            var eventMap = eventBuilder.Build(new ProjectorMap<TProjector, TKey, ProjectionContext>
            {
                Create = async (key, context, pro, overwite) =>
                {
                    var projection = new TProjector();

                    await pro(projection);

                    if (overwite(projection))
                        store.AddOrUpdate(key, _ => projection, (o, _) => projection);
                    else if (!store.TryAdd(key, projection))
                        throw new InvalidOperationException("Error On Add Projection");
                },

                Delete = (key, context) => Task.FromResult(store.TryRemove(key, out _)),

                Update = async (key, context, pro, missing) =>
                {
                    if (!store.TryGetValue(key, out var projection))
                    {
                        if (!missing())
                            return;

                        projection = store.GetOrAdd(key, id => new TProjector{ Id = id });
                    }

                    await pro(projection);
                },

                Custom = (context, pro) => pro()
            });

            var projector = new Projector(eventMap)
            {
                ShouldRetry = async (exception, attempts) =>
                {
                    await Task.Delay((int)Math.Pow(2d, attempts));

                    return (attempts < 3);
                }
            };

            var options = new SubscriptionOptions
                          {
                              Id = tag,
                              RestartWhenAhead = true,
                              BeforeRestarting = () =>
                              {
                                  store.Clear();
                                  return Task.CompletedTask;
                              }
                          };

            var dispatcher = new Dispatcher(_eventStoreReader.CreateSubscription)
                             {
                                 ExceptionHandler = (exception, attempts, info) 
                                     => !exception.IsCriticalApplicationException() && attempts < 3 ? Task.FromResult(ExceptionResolution.Retry) : Task.FromResult(ExceptionResolution.Abort)
                             };

            return dispatcher.Subscribe(0, async (transactions, info) => await projector.Handle(transactions), options);
        }
    }
}