using System;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public interface IInitableActorRef
    {
        IObservable<IActorRef> Actor { get; }

        void Init(string? name = null);

        void Init(IActorRefFactory factory, string? name = null);
    }
}