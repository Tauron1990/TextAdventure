using System;
using System.Reactive.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;

namespace TextAdventures.Engine.EventSystem
{
    [PublicAPI]
    public sealed class EventDispatcher
    {
        public EventDispatcher(IActorRef dispatcher)
            => Dispatcher = dispatcher;

        public IActorRef Dispatcher { get; }

        public IObservable<GenericEventResponse<TEvent>> Event<TEvent>()
            => Dispatcher.Ask<GenericEventResponse<TEvent>>(new GenericEventRequest<TEvent>()).ToObservable();

        public IObservable<AllEventsResponse> AllEvent()
            => Dispatcher.Ask<AllEventsResponse>(new RequestAllEvents()).ToObservable();

        public void Send(object msg)
            => Dispatcher.Tell(msg);
    }
}