using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Event;
using Tauron.Akka;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class LoadingManagerActor : ExposedReceiveActor, IWithTimers
    {
        private int _currentCount;
        private readonly List<Action> _waiter = new List<Action>(); 

        public LoadingManagerActor()
        {
            Receive<WaitUntilLoaded>(reg =>
            {
                if(_currentCount == 0)
                    Self.Tell(reg.OnLoaded);
                else
                    _waiter.Add(reg.OnLoaded);
            });

            Receive<Decrementer>(d =>
            {
                if(Timers.IsTimerActive(d))
                    Timers.Cancel(d);

                if(_currentCount == 0)
                    return;
                
                _currentCount--;
                if(_currentCount != 0)
                    return;

                foreach (var action in _waiter) 
                    Self.Tell(action);

                _waiter.Clear();
            });

            Receive<Action>(a =>
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Context.GetLogger().Error(e, "Error on Loading Compled");
                }
            });

            Receive<LoadingSequence>(ls =>
            {
                _currentCount++;

                var registration = new Decrementer(Self);

                if (ls.Timeout != null && ls.Timeout != Timeout.InfiniteTimeSpan) 
                    Timers.StartSingleTimer(registration, registration, ls.Timeout.Value);

                ls.Compledtion.SetResult(registration);
            });
        }

        public ITimerScheduler Timers { get; set; } = null!;

        private sealed class Decrementer : IDisposable
        {
            private IActorRef _run;

            public Decrementer(IActorRef run) 
                => _run = run;

            public void Dispose()
            {
                _run.Tell(this);
                _run = ActorRefs.Nobody;
            }
        }
    }
}