using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Actors;
using Akka.Streams.Dsl;
using JetBrains.Annotations;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine.EventSystem
{
    public sealed class EventDispatcher : GameProcess
    {
        public EventDispatcher()
        {
            ActorMaterializer materializer = Context.Materializer();
            
            var source = Source.ActorPublisher<GameEvent>(Props.Create<EventSender>());
            var sink = BroadcastHub.Sink<GameEvent>();

            var (publisher, broadcastSource) = source.ToMaterialized(sink, Keep.Both).Run(materializer);


            Receive<RequestEventSource>(r => Sender.Tell(r.Create(broadcastSource)));
            ReceiveAny(o => publisher.Forward(o));
        }
        
        private sealed class EventSender : ActorPublisher<GameEvent>, IWithTimers
        {
            public ITimerScheduler Timers { get; set; } = null!;
            
            private readonly Queue<GameEvent> _pendingEvents = new();

            protected override bool Receive(object message)
            {
                if (message is TrySend)
                {
                    TrySendImpl();
                    return true;
                }

                GameEvent gameEvent;
                if (message is GameEvent evt)
                    gameEvent = evt;
                else
                    gameEvent = new GameEvent(message);

                if(TotalDemand > 0)
                    OnNext(gameEvent);
                else
                {
                    _pendingEvents.Enqueue(gameEvent);
                    StartTimer();
                }

                return true;
            }

            private void TrySendImpl()
            {
                if(_pendingEvents.Count == 0)
                    return;

                if (TotalDemand == 0)
                {
                    StartTimer();
                    return;
                }

                var demand = TotalDemand;
                for (int i = 0; i < demand && _pendingEvents.Count != 0; i++) 
                    OnNext(_pendingEvents.Dequeue());
                
                if(_pendingEvents.Count != 0)
                    StartTimer();
            }
            
            private void StartTimer() => Timers.StartSingleTimer(nameof(TrySend), TrySend.Instance, TimeSpan.FromSeconds(1));

            private sealed record TrySend()
            {
                public static readonly TrySend Instance = new();
            }
        }
    }
    
    public sealed record GameEvent(object ActualEvent);

    public abstract record RequestEventSource
    {
        public abstract EventSourceResponse Create(Source<GameEvent, NotUsed> source);
    }

    public abstract record EventSourceResponse;

    [PublicAPI]
    public sealed record GenericEventRequest<TEvent> : RequestEventSource {
        public override EventSourceResponse Create(Source<GameEvent, NotUsed> source)
        {
            return new GenericEventResponse<TEvent>(source
                                                   .Where(e => e.ActualEvent is TEvent)
                                                   .Select(e => (TEvent) e.ActualEvent));
        }
    }

    [PublicAPI]
    public sealed record GenericEventResponse<TEvent>(Source<TEvent, NotUsed> Source) : EventSourceResponse;
    
    [PublicAPI]
    public sealed record RequestAllEvents : RequestEventSource
    {
        public override EventSourceResponse Create(Source<GameEvent, NotUsed> source) 
            => new AllEventsResponse(source);
    }
    
    [PublicAPI]
    public sealed record AllEventsResponse(Source<GameEvent, NotUsed> Events) : EventSourceResponse;
}