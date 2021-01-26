﻿using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using Tauron.Application.Workshop.Core;

namespace Tauron.Application.Workshop.Mutation
{
    [PublicAPI]
    public abstract class EventSourceBase<TRespond> : DeferredActor, IEventSource<TRespond>
    {
        private readonly Subject<TRespond> _subject = new();
        private readonly WorkspaceSuperviser _superviser;

        protected EventSourceBase(Task<IActorRef> mutator, WorkspaceSuperviser superviser)
            : base(mutator)
            => _superviser = superviser;

        public IDisposable RespondOn(IActorRef actorRef)
        {
            var dispo = _subject.Subscribe(n => actorRef.Tell(n));
            _superviser.WatchIntrest(new WatchIntrest(dispo.Dispose, actorRef));
            return dispo;
        }

        public IDisposable RespondOn(IActorRef? source, Action<TRespond> action)
        {
            if (source.IsNobody())
                return _subject.Subscribe(action);

            var dispo = _subject.Subscribe(t => source.Tell(IncommingEvent.From(t, action)));
            _superviser.WatchIntrest(new WatchIntrest(dispo.Dispose, source!));
            return dispo;
        }

        public IDisposable Subscribe(IObserver<TRespond> observer) => _subject.Subscribe(observer);

        protected IObserver<TRespond> Sender() => _subject.AsObserver();
    }
}