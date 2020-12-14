using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public abstract class BaseActorRef<TActor> : IDisposable
        where TActor : ActorBase
    {
        private readonly ActorRefFactory<TActor> _builder;
        private readonly BehaviorSubject<IActorRef> _actor = new(Nobody.Instance);
        
        protected BaseActorRef(ActorRefFactory<TActor> actorBuilder)
            => _builder = actorBuilder;

        public IObservable<bool> IsInitialized 
            => _actor.Select(a => !a.IsNobody());

        protected virtual bool IsSync => false;

        public IObservable<IActorRef> Actor 
            => _actor.AsObservable();

        public IObservable<ActorPath> Path
            => from act in Actor
               select act.Path;
        
        public void Tell(object message, IActorRef sender) => _actor.Value.Tell(message, sender);

        public bool Equals(IActorRef? other) 
            => _actor.Value.Equals(other);

        public int CompareTo(IActorRef? other)
            => _actor.Value.CompareTo(other);

        public int CompareTo(object? obj)
            => _actor.Value.CompareTo(obj);

        public virtual void Init(string? name = null)
            => IniCore((b, s) => _builder.Create(b, s), name);

        public virtual void Init(IActorRefFactory factory, string? name = null) 
            => IniCore((sync, parmName) => factory.ActorOf(_builder.CreateProps(sync), parmName), name);

        protected virtual void IniCore(Func<bool, string?, IActorRef> create, string? name) 
            => _actor.OnNext(create(IsSync, name));

        protected void ResetInternal() 
            => _actor.OnNext(Nobody.Instance);
        
        public void Dispose()
        {
            _actor.OnCompleted();
            _actor.Dispose();
        }
    }
}