using System;
using System.Reactive.Linq;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public static class InitableExtensions
    {
        public static IObservable<TResult> Ask<TResult>(this IInitableActorRef model, object message, TimeSpan? timeout = null) 
            => model.Actor.NotNobody().FirstAsync().SelectMany(a => a.Ask<TResult>(message, timeout));

        public static void Tell(this IInitableActorRef model, object msg)
            => Tell(model, msg, ActorCell.GetCurrentSenderOrNoSender());

        public static void Tell(this IInitableActorRef model, object msg, IActorRef sender) 
            => model.Actor.NotNobody().SingleTimeSubscripe(evt => evt.Tell(msg, sender));
    }
}