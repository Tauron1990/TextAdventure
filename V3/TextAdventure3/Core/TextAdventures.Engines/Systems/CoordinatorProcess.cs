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
    public abstract class CoordinatorProcess<TState> : GameProcess<TState>
    {
        private static readonly MethodInfo ReceiveConsumeMethod = typeof(CoordinatorProcess<TState>).GetMethod(nameof(ReceiveConsume), BindingFlags.Instance | BindingFlags.NonPublic)!;
        
        protected override void Config()
        {
            base.Config();
            var materializer = Context.Materializer();

            foreach (var @interface in GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IConsumeEvent<,>))) 
                ReceiveConsumeMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]).Invoke(this, new object?[] {this, materializer});
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
        private void ReceiveConsume<T>(IConsumeEvent<T, TState> self, ActorMaterializer materializer)
        {
            Receive<T>(obs => self.Process(obs));

            Game.EventDispatcher.Event<T>()
                .ContinueWith(ge =>
                              {
                                  var sink = Sink.ActorRef<T>(Self, PoisonPill.Instance);
                                  ge.Result.Source.ToMaterialized(sink, Keep.Left).Run(materializer);
                              });
        }
    }
}