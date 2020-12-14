using System;
using System.Linq.Expressions;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public static class ActorRefFactoryExtensions
    {
        public static IActorRef GetOrAdd<TActor>(this IActorContext context, string name) where TActor : ActorBase, new()
            => GetOrAdd(context, Props.Create<TActor>(), name);

        public static IActorRef GetOrAdd(this IActorContext context, Props props, string name)
        {
            var child = context.Child(name);
            return child.IsNobody() ? context.ActorOf(props, name) : child;
        }

        public static IActorRef ActorOf<TActor>(this IActorRefFactory fac, Expression<Func<TActor>> creator, string? name) where TActor : ActorBase
            => fac.ActorOf(Props.Create(creator), name);
    }
}