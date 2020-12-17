using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.Event;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    public interface IExposedReceiveActor
    {
        IActorDsl Exposed { get; }
    }

    [PublicAPI]
    public class ExposedReceiveActor : ReceiveActor, IActorDsl, IExposedReceiveActor
    {
        private readonly List<IDisposable> _resources = new();
        private Action<Exception, IActorContext>? _onPostRestart;
        private Action<IActorContext>? _onPostStop;
        private Action<Exception, object, IActorContext>? _onPreRestart;
        private Action<IActorContext>? _onPreStart;
        private SupervisorStrategy? _strategy;

        public IActorDsl Exposed => this;

        public static IUntypedActorContext ExposedContext => Context;

        public ExposedReceiveActor()
        {
            Receive<TransmitError>(e => e.ErrorHandler(e.Error));
        }
        
        public void AddResource(IDisposable res)
            => _resources.Add(res);

        protected internal ILoggingAdapter Log { get; } = Context.GetLogger();

        #region ActorDsl

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

        protected event Action<Exception>? OnPostRestart;

        protected event Action<Exception, object>? OnPreRestart;

        protected event Action? OnPostStop;

        protected event Action? OnPreStart;

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

        #endregion

        protected void ObservableReceiveSafe<TEvent>(Func<IObservable<TEvent>, IObservable<Unit>> handler) 
            => AddResource(new ObservableInvoker<TEvent, Unit>(handler, DefaultError, this, Self).Construct());

        protected void ObservableReceiveSafe<TEvent>(Func<IObservable<TEvent>, IObservable<TEvent>> handler) 
            => AddResource(new ObservableInvoker<TEvent, TEvent>(handler, DefaultError, this, Self).Construct());

        protected void WhenReceive<TEvent>(Func<IObservable<TEvent>, IObservable<Unit>> handler)
            => AddResource(new ObservableInvoker<TEvent, Unit>(handler, ThrowError, this, Self).Construct());

        protected void WhenReceive<TEvent>(Func<IObservable<TEvent>, IObservable<TEvent>> handler)
            => AddResource(new ObservableInvoker<TEvent, TEvent>(handler, ThrowError, this, Self).Construct());

        private bool ThrowError(Exception e)
        {
            Log.Error(e, "Error on Process Event");
            throw e;
        }

        private bool DefaultError(Exception e)
        {
            Log.Error(e, "Error on Process Event");
            return true;
        }

        /*protected static bool CallSafe(Action exec, Action<Exception>? catching = null, Action? finalizing = null)
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

        protected static Action<TMsg> When<TMsg>(Func<TMsg, bool> test, Action<TMsg> action)
        {
            return m =>
                   {
                       if (test(m))
                           action(m);
                   };
        }*/

        private sealed class ObservableInvoker<TEvent, TResult> : IDisposable
        {
            private Subject<TEvent>? _source;
            private IDisposable? _subscription;
            
            private readonly Func<IObservable<TEvent>, IObservable<TResult>> _factory;
            private readonly Func<Exception, bool> _errorHandler;
            private readonly IActorDsl _dsl;
            private readonly IActorRef _self;

            public ObservableInvoker(Func<IObservable<TEvent>, IObservable<TResult>> factory, Func<Exception, bool> errorHandler, IActorDsl dsl, IActorRef self)
            {
                _factory = factory;
                _errorHandler = errorHandler;
                _dsl = dsl;
                _self = self;
                
                Init();
            }

            public IDisposable Construct()
            {
                _dsl.Receive<TEvent>(Runner);
                return this;
            }

            private void Runner(TEvent @event, IActorContext actorContext) 
                => _source?.OnNext(@event);

            private void Init()
            {
                _source = new Subject<TEvent>();
                _subscription = _factory(_source.AsObservable())
                   .Subscribe(_ => { }, e =>
                                        {
                                            _self.Forward(new TransmitError(e, exception =>
                                                                               {
                                                                                   _source?.Dispose();
                                                                                   _subscription?.Dispose();
                                                                                   _source = null;
                                                                                   _subscription = null;

                                                                                   if (_errorHandler(exception))
                                                                                       Init();
                                                                               }));
                                        });
            }
            
            void IDisposable.Dispose()
            {
                _source?.Dispose();
                _subscription?.Dispose();
            }
        }

        private sealed record TransmitError(Exception Error, Action<Exception> ErrorHandler);

    }
}