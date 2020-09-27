using System;
using Akka.Actor;

namespace TextAdventures.Engine.Internal
{
    public sealed class WaitUntilLoaded
    {
        public Action OnLoaded { get; }

        private WaitUntilLoaded(Action onLoaded) 
            => OnLoaded = onLoaded;

        public static WaitUntilLoaded New(Action then)
            => new WaitUntilLoaded(then);

        public static WaitUntilLoaded New(IActorRef target, object message)
            => new WaitUntilLoaded(() => target.Tell(message, ActorCell.GetCurrentSelfOrNoSender()));
    }
}