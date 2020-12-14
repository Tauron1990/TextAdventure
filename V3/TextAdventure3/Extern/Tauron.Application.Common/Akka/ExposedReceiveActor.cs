﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.Event;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public interface IExposedReceiveActor
    {
        public IActorDsl Exposed { get; }

        void WhenMessageReceive<TMessage>(Func<IObservable<TMessage>, IDisposable> handler);

        IObservable<TMessage> WhenMessageReceive<TMessage>();
    }

    [PublicAPI]
    public class ExposedReceiveActor : ReceiveActor, IActorDsl, IExposedReceiveActor
    {
        private readonly List<IDisposable>                         _resources = new();
        private          Action<Exception, IActorContext>?         _onPostRestart;
        private          Action<IActorContext>?                    _onPostStop;
        private          Action<Exception, object, IActorContext>? _onPreRestart;
        private          Action<IActorContext>?                    _onPreStart;
        private          SupervisorStrategy?                       _strategy;

        public static IUntypedActorContext ExposedContext => Context;

        protected internal ILoggingAdapter Log { get; } = Context.GetLogger();

        #region IActorDsl
        
        void IActorDsl.Receive<T>(Action<T, IActorContext> handler) => Receive<T>(m => handler(m, Context));

        void IActorDsl.Receive<T>(Predicate<T> shouldHandle, Action<T, IActorContext> handler) => Receive(shouldHandle, obj => handler(obj, Context));

        void IActorDsl.Receive<T>(Action<T, IActorContext> handler, Predicate<T> shouldHandle) => Receive(t => handler(t, Context), shouldHandle);

        void IActorDsl.ReceiveAny(Action<object, IActorContext> handler) => ReceiveAny(m => handler(m, Context));

        void IActorDsl.ReceiveAsync<T>(Func<T, IActorContext, Task> handler, Predicate<T> shouldHandle) => ReceiveAsync(m => handler(m, Context), shouldHandle);

        void IActorDsl.ReceiveAsync<T>(Predicate<T> shouldHandle, Func<T, IActorContext, Task> handler) => ReceiveAsync(shouldHandle, arg => handler(arg, Context));

        void IActorDsl.ReceiveAnyAsync(Func<object, IActorContext, Task> handler) => ReceiveAnyAsync(m => handler(m, Context));

        void IActorDsl.DefaultPreRestart(Exception reason, object message) => base.PreRestart(reason, message);

        void IActorDsl.DefaultPostRestart(Exception reason) => PostRestart(reason);

        void IActorDsl.DefaultPreStart() => base.PreStart();

        void IActorDsl.DefaultPostStop() => base.PostStop();

        void IActorDsl.Become(Action<object, IActorContext> handler) => Become(o => handler(o, Context));

        void IActorDsl.BecomeStacked(Action<object, IActorContext> handler) => BecomeStacked(o => handler(o, Context));

        void IActorDsl.UnbecomeStacked() => UnbecomeStacked();

        IActorRef IActorDsl.ActorOf(Action<IActorDsl> config, string name) => Context.ActorOf(config, name);

        Action<Exception, IActorContext>? IActorDsl.OnPostRestart
        {
            get => _onPostRestart;
            set => _onPostRestart = (Action<Exception, IActorContext>?) Delegate.Combine(_onPostRestart, value);
        }

        Action<Exception, object, IActorContext>? IActorDsl.OnPreRestart
        {
            get => _onPreRestart;
            set => _onPreRestart = (Action<Exception, object, IActorContext>?) Delegate.Combine(_onPostRestart, value);
        }

        Action<IActorContext>? IActorDsl.OnPostStop
        {
            get => _onPostStop;
            set => _onPostStop = (Action<IActorContext>?) Delegate.Combine(_onPostStop, value);
        }

        Action<IActorContext>? IActorDsl.OnPreStart
        {
            get => _onPreStart;
            set => _onPreStart = (Action<IActorContext>?) Delegate.Combine(_onPreStart, value);
        }

        SupervisorStrategy? IActorDsl.Strategy
        {
            get => _strategy;
            set => _strategy = value;
        }

        #endregion

        public IActorDsl Exposed => this;

        //public void Flow<TStart>(Action<ActorFlowBuilder<TStart>> builder)
        //    => builder(new ActorFlowBuilder<TStart>(this));

        //public EnterFlow<TStart> EnterFlow<TStart>(Action<ActorFlowBuilder<TStart>> builder)
        //{
        //    var flowBuilder = new ActorFlowBuilder<TStart>(this);
        //    builder(flowBuilder);
        //    return flowBuilder.Build();
        //}

        public void AddResource(IDisposable res)
            => _resources.Add(res);

        protected event Action<Exception>? OnPostRestart;

        protected event Action<Exception, object>? OnPreRestart;

        protected event Action? OnPostStop;

        protected event Action? OnPreStart;

        protected override void PostRestart(Exception reason)
        {
            _onPostRestart?.Invoke(reason, Context);
            OnPostRestart?.Invoke(reason);
            base.PostRestart(reason);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _onPreRestart?.Invoke(reason, message, Context);
            OnPreRestart?.Invoke(reason, message);
            base.PreRestart(reason, message);
        }

        protected override void PostStop()
        {
            _onPostStop?.Invoke(Context);
            OnPostStop?.Invoke();

            foreach (var disposable in _resources)
                disposable.Dispose();

            base.PostStop();
        }

        protected override void PreStart()
        {
            _onPreStart?.Invoke(Context);
            OnPreStart?.Invoke();
            base.PreStart();
        }

        protected override SupervisorStrategy SupervisorStrategy() => _strategy ?? base.SupervisorStrategy();

        protected static bool CallSafe(Action exec, Action<Exception>? catching = null, Action? finalizing = null)
        {
            try
            {
                exec();
                return true;
            }
            catch (Exception e)
            {
                catching?.Invoke(e);
                return false;
            }
            finally
            {
                finalizing?.Invoke();
            }
        }

        protected bool CallSafe(Action exec, string logMessage, Action finalizing)
        {
            try
            {
                exec();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, logMessage);
                return false;
            }
            finally
            {
                finalizing.Invoke();
            }
        }

        protected bool CallSafe(Action exec, string logMessage)
        {
            try
            {
                exec();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, logMessage);
                return false;
            }
        }

        protected void CallSafe(Action exec, string logMessage, Action<bool> finalizing)
        {
            var error = false;
            try
            {
                exec();
            }
            catch (Exception e)
            {
                error = true;
                Log.Error(e, logMessage);
            }
            finally
            {
                finalizing.Invoke(error);
            }
        }

        public IObservable<TMessage> WhenMessageReceive<TMessage>()
        {
            var responder = new Subject<TMessage>();
            AddResource(responder);
            
            Receive<TMessage>(m => responder.OnNext(m));
            
            return responder.AsObservable();
        }

        public void WhenMessageReceive<TMessage>(Func<IObservable<TMessage>, IDisposable> handler) 
            => AddResource(handler(WhenMessageReceive<TMessage>()));

        //protected static Action<TMsg> When<TMsg>(Func<TMsg, bool> test, Action<TMsg> action)
        //{
        //    return m =>
        //           {
        //               if (test(m))
        //                   action(m);
        //           };
        //}
    }
}