using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;

namespace TextAdventures.Engine.EventSystem
{
    [PublicAPI]
    public sealed class EventDispatcher
    {
        public IActorRef Dispatcher { get; }

        public EventDispatcher(IActorRef dispatcher)
            => Dispatcher = dispatcher;

        public Task<GenericEventResponse<TEvent>> Event<TEvent>()
            => Dispatcher.Ask<GenericEventResponse<TEvent>>(new GenericEventRequest<TEvent>());

        public Task<AllEventsResponse> AllEvent()
            => Dispatcher.Ask<AllEventsResponse>(new RequestAllEvents());

        public void Send(object msg)
            => Dispatcher.Tell(msg);
    }
}