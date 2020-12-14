﻿using Akka.Actor;
using Akka.DI.Core;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public sealed class ActorRefFactory<TActor>
    {
        private readonly ActorSystem _system;

        public ActorRefFactory(ActorSystem system)
            => _system = system;

        public IActorRef Create(bool sync, string? name = null)
            => _system.ActorOf(CreateProps(sync), name);

        public Props CreateProps(bool sync)
        {
            var prop = _system.DI().Props(typeof(TActor));
            return sync ? prop.WithDispatcher("synchronized-dispatcher") : prop;
        }
    }
}