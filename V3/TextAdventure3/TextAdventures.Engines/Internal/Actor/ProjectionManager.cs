using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Core;
using JetBrains.Annotations;
using LiquidProjections;
using Tauron;
using Tauron.Akka;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Events;
using TextAdventures.Engine.Internal.Data.Projection;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Internal.Querys;
using TextAdventures.Engine.Querys;
using TextAdventures.Engine.Querys.Result;
using TextAdventures.Engine.Querys.Room;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class ProjectionManager : ExposedReceiveActor
    {
        private static readonly MethodInfo ConstructProjectorMethodInfo = typeof(ProjectionManager).GetMethod("ConstructProjector", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly GameProjection _gameProjection = new GameProjection();
        private readonly EventStoreReader _eventStoreReader = new EventStoreReader(Context.System);
        private readonly List<IDisposable> _disposable = new List<IDisposable>();
        private readonly Dictionary<Type, IQueryHandler> _queryHandlers = new Dictionary<Type, IQueryHandler>();

        public ProjectionManager()
        {
            Receive<IGameQuery>(q =>
            {
                if (!_queryHandlers.TryGetValue(q.Target, out var handler))
                    Sender.Tell(QueryResult.NotFound());
                else
                    handler.Handle(q, Sender);
            });
            Receive<GameProjectionQuery>(g => Sender.Tell(QueryResult.Compleded(_gameProjection)));
            Receive<INewProjector>(np =>
            {
                _queryHandlers[np.Target] = np.Handler;

                var targetDelegate = typeof(Func<,,,>)
                   .MakeGenericType(
                        typeof(string),
                        typeof(ConcurrentDictionary<,>).MakeGenericType(np.Key, np.Projector),
                        typeof(Action<>).MakeGenericType(typeof(EventMapBuilder<,,>).MakeGenericType(np.Projector, np.Key, typeof(ProjectionContext))),
                        typeof(IDisposable));

                var met = ConstructProjectorMethodInfo.MakeGenericMethod(np.Key, np.Projector);

                np.Install(Delegate.CreateDelegate(targetDelegate, this, met));
            });
            Receive<INewQueryHandler>(h => _queryHandlers[h.Target] = h.Handler);
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
            Self.Tell(new AddProjection<RoomId, RoomProjection, RoomQueryHandler, RoomQueryBase>(
                new RoomQueryHandler(_gameProjection.Rooms), "Room", disposable => _disposable.Add(disposable),
                eventBuilder =>
                {
                    eventBuilder
                       .Map<RoomCreatedEvent>()
                       .AsCreateOf(e => e.Id)
                       .IgnoringDuplicates()
                       .Using((projection, createdEvent) => projection.Id = createdEvent.Id);
                }), Self);

        }

        [UsedImplicitly]
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