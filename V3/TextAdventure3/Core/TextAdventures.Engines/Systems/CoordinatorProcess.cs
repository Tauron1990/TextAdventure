using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using JetBrains.Annotations;
using Tauron.Akka;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Modules.Text;

namespace TextAdventures.Engine.Systems
{
    [PublicAPI]
    public abstract class CoordinatorProcess<TState> : GameProcess<TState>
    {
        private static readonly MethodInfo ReceiveConsumeMethod = typeof(CoordinatorProcess<TState>).GetMethod(nameof(ReceiveConsume), BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly ConcurrentDictionary<Type, object> _components = new();
        private readonly ConcurrentDictionary<string, IObservable<GameObject>> _objects = new();

        protected override void Config()
        {
            base.Config();
            var materializer = Context.Materializer();

            foreach (var @interface in GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IConsumeEvent<,>)))
                ReceiveConsumeMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]).Invoke(this, new object?[] {this, materializer});
        }

        public IObservable<GameObject> GetObject(string name)
            => _objects.GetOrAdd(name, _ => Game.ObjectManager.GetObject(name)
                                                .Select(o =>
                                                        {
                                                            if (o == null)
                                                                throw new InvalidOperationException("Object Not Found");
                                                            return o;
                                                        }));

        public IObservable<TType> GetGlobalComponent<TType>()
            where TType : class
            => (IObservable<TType>) _components.GetOrAdd(typeof(TType),
                                                         _ => Game.ObjectManager
                                                                  .GetGlobalComponent<TType>()
                                                                  .Select(c =>
                                                                          {
                                                                              if (c == null)
                                                                                  throw new InvalidOperationException("Component Not Found");

                                                                              return c;
                                                                          }));
        
        protected void EmitEvents(params object[] events)
        {
            foreach (var @event in events)
                Game.EventDispatcher.Send(@event);
        }

        protected void EmitEvents(ComponentBase component, params object[] events)
        {
            foreach (var @event in events)
            {
                component.ApplyEvent(@event);
                Game.EventDispatcher.Send(@event);
            }
        }

        protected void SendCommand(IGameCommand command)
            => Game.ObjectManager.Dispatch(command);

        [UsedImplicitly]
        private void ReceiveConsume<T>(IConsumeEvent<T, TState> self, ActorMaterializer materializer)
        {
            Receive<T>(self.Process);

            Game.EventDispatcher.Event<T>()
                .ObserveOnSelf()
                .Subscribe(ge =>
                              {
                                  var sink = Sink.ActorRef<T>(Self, PoisonPill.Instance);
                                  ge.Source.ToMaterialized(sink, Keep.Left).Run(materializer);
                              });
        }
    }
}