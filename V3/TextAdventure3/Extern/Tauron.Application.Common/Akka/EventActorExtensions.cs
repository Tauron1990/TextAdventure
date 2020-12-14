﻿using System;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public static class EventActorExtensions
    {
        public static IEventActor CreateEventActor(this IActorRefFactory system, bool killOnFirstResponse = false)
            => EventActor.Create(system, null, killOnFirstResponse);

        public static IEventActor CreateEventActor<TPayload>(this IActorRefFactory system, Action<TPayload> handler, bool killOnFirstResponse = false)
            => EventActor.Create(system, handler, killOnFirstResponse);

        public static IEventActor CreateEventActor(this IActorRefFactory system, string? name = null, bool killOnFirstResponse = false)
            => EventActor.Create(system, name, killOnFirstResponse);

        public static IEventActor GetOrCreateEventActor(this IUntypedActorContext system, string? name = null, bool killOnFirstResponse = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                return EventActor.Create(system, killOnFirstResponse:killOnFirstResponse);
            
            var child = system.Child(name);
            return child.IsNobody() 
                       ? EventActor.Create(system, name, killOnFirstResponse) 
                       : EventActor.From(child);
        }
    }
}