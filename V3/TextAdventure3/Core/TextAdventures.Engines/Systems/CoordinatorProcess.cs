using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using JetBrains.Annotations;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Systems
{
    [PublicAPI]
    public abstract class CoordinatorProcess : GameProcess
    {
        private static readonly MethodInfo ReceiveConsumeMethod = typeof(CoordinatorProcess).GetMethod(nameof(ReceiveConsume), BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected CoordinatorProcess()
        {
            foreach (var @interface in GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IConsumeEvent<>)))
            {
                var materializer = Context.Materializer();
                ReceiveConsumeMethod.MakeGenericMethod(@interface.GenericTypeArguments).Invoke(this, new object?[] {this, materializer});

                Receive<Status.Failure>(f => throw f.Cause);
            }
        }

        public Task<GameObject> GetObject(string name) 
            => Game.ObjectManager.GetObject(name)
                   .ContinueWith(t => t.IsCompletedSuccessfully && t.Result != null ? t.Result : throw new InvalidOperationException("Object Not Found"));

        public Task<TType> GetGlobalComponent<TType>()
            where TType : class => Game.ObjectManager.GetGlobalComponent<TType>()
                                       .ContinueWith(t => t.IsCompletedSuccessfully && t.Result != null ? t.Result : throw new InvalidOperationException("Component Not Found"));

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
        private void ReceiveConsume<T>(IConsumeEvent<T> self, ActorMaterializer materializer)
        {
            Receive<T>(self.Process);

            Game.EventDispatcher.Event<T>()
                .ContinueWith(ge =>
                              {
                                  var sink = Sink.ActorRef<T>(Self, PoisonPill.Instance);
                                  ge.Result.Source.ToMaterialized(sink, Keep.Left).Run(materializer);
                              });
        }
    }
}