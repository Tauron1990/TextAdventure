using System;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public sealed class EventActor : UntypedActor
    {
        private readonly bool            _killOnFirstRespond;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private ImmutableDictionary<Type, Delegate> _registrations = ImmutableDictionary<Type, Delegate>.Empty;
        private ImmutableList<IDisposable> _resource = ImmutableList<IDisposable>.Empty;
        
        private EventActor(bool killOnFirstRespond) => _killOnFirstRespond = killOnFirstRespond;

        public static IEventActor From(IActorRef actorRef)
            => new HookEventActor(actorRef);

        public static IEventActor Create(IActorRefFactory system, string? name = null, bool killOnFirstResponse = false)
        {
            var actor = system.ActorOf(Props.Create(() => new EventActor(killOnFirstResponse)), name);
            return new HookEventActor(actor);
        }

        public static IEventActor Create<TPayload>(IActorRefFactory system, Action<TPayload> handler, bool killOnFirstResponse = false)
        {
            var actor = Create(system, null, killOnFirstResponse);
            actor.Register(HookEvent.Create(handler));
            return actor;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HookEvent hookEvent:
                    _registrations = _registrations.TryGetValue(hookEvent.Target, out var del) 
                                         ? _registrations.SetItem(hookEvent.Target, del.Combine(hookEvent.Invoker)) 
                                         : _registrations.Add(hookEvent.Target, hookEvent.Invoker);
                    if (hookEvent.Disposable != null)
                        _resource = _resource.Add(hookEvent.Disposable);
                    break;
                default:
                    try
                    {
                        if (_registrations.TryGetValue(message.GetType(), out var handler))
                        {
                            handler.DynamicInvoke(message);
                            
                            if (_killOnFirstRespond)
                                Context.Stop(Self);
                        }
                        else
                            Unhandled(message);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "Error On Event Hook Execution");
                    }
                    break;
            }
        }

        protected override void PostStop()
        {
            foreach (var res in _resource) 
                res.Dispose();
            base.PostStop();
        }

        private sealed class HookEventActor : IEventActor
        {
            public HookEventActor(IActorRef actorRef)
                => OriginalRef = actorRef;

            public IActorRef OriginalRef { get; }

            public IEventActor Register(HookEvent hookEvent)
            {
                OriginalRef.Tell(hookEvent);
                return this;
            }

            public IEventActor Send(IActorRef actor, object send)
            {
                actor.Tell(send, OriginalRef);
                return this;
            }
        }
    }
}