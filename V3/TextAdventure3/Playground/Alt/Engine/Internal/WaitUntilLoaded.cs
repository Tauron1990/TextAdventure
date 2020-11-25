using System;
using Akka.Actor;

namespace TextAdventures.Engine.Internal
{
    public sealed class WaitUntilLoaded
    {
        private WaitUntilLoaded(Action onLoaded)
            => OnLoaded = onLoaded;

        public Action OnLoaded { get; }

        public static WaitUntilLoaded New(Action then)
            => new(then);

        public static WaitUntilLoaded New(IActorRef target, object message)
            => new(() => target.Tell(message, ActorCell.GetCurrentSelfOrNoSender()));
    }
}