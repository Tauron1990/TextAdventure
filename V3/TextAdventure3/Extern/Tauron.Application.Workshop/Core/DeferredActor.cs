﻿using System.Collections.Immutable;
using System.Threading.Tasks;
using Akka.Actor;

namespace Tauron.Application.Workshop.Core
{
    public abstract class DeferredActor
    {
        private ImmutableList<object>? _stash;

        protected DeferredActor(Task<IActorRef> actor)
        {
            actor.ContinueWith(OnCompleded);
            _stash = ImmutableList<object>.Empty;
        }

        private IActorRef Actor { get; set; } = ActorRefs.Nobody;

        private void OnCompleded(Task<IActorRef> obj)
        {
            lock (this)
            {
                Actor = obj.Result;
                foreach (var message in _stash ?? ImmutableList<object>.Empty)
                    Actor.Tell(message);

                _stash = null;
            }
        }

        protected void TellToActor(object msg)
        {
            if (!Actor.IsNobody())
                Actor.Tell(msg);
            else
            {
                lock (this)
                {
                    if (!Actor.IsNobody())
                    {
                        Actor.Tell(msg);
                        return;
                    }

                    _stash = _stash?.Add(msg) ?? ImmutableList<object>.Empty.Add(msg);
                }
            }
        }
    }
}