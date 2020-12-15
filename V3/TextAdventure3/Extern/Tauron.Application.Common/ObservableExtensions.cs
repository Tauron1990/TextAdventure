using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Akka.Actor;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class ObservableExtensions
    {
        //private sealed class SingleCallGate<TEvent> : IObservable<TEvent>, IObserver<TEvent>
        //{
        //    private readonly object _gate = new();
        //    private ImmutableList<IObserver<TEvent>> _observers = ImmutableList<IObserver<TEvent>>.Empty;

        //    public IDisposable Subscribe(IObserver<TEvent> observer)
        //    {
        //        lock (_gate)
        //            _observers = _observers.Add(observer);

        //    }

        //    public void OnCompleted() { }
        //    public void OnError(Exception error) { }
        //    public void OnNext(TEvent value) { }
        //}

        //public static IObservable<TEvent> SingleCall<TEvent>(this IObservable<TEvent> observable)
        //{

        //}

        private sealed class SingleTimeObserver<TEvent> : IObserver<TEvent>, IDisposable
        {
            private readonly object _gate = new();
            private IDisposable? _dis;
            private int _runed;

            private Action<TEvent>? Handler { get; }

            public Action<Exception>? Error { get; init; }

            public Action? OnCompled { get; init; }
            
            public SingleTimeObserver(Action<TEvent> handler) => Handler = handler;

            public SingleTimeObserver()
            {
                
            }

            public IDisposable Register(IObservable<TEvent> evt)
            {
                lock (_gate)
                {
                    _dis = evt.Subscribe(this);
                    if (_runed == 1)
                        _dis?.Dispose();
                }

                return _dis ?? Disposable.Empty;
            }
            
            public void OnCompleted()
                => OnCompled?.Invoke();

            public void OnError(Exception error)
                => Error?.Invoke(error);

            public void OnNext(TEvent value)
            {
                Handler?.Invoke(value);

                lock (_gate)
                {
                    _dis?.Dispose();
                    _dis = null;
                    _runed = 1;
                }
            }

            public void Dispose() 
                => _dis?.Dispose();
        }

        public static IDisposable SingleTimeSubscribe<TEvent>(this IObservable<TEvent> observable)
        {
            var observer = new SingleTimeObserver<TEvent>();

            return observer.Register(observable);
        }

        public static IDisposable SingleTimeSubscribe<TEvent>(this IObservable<TEvent> observable, Action<TEvent> handler, Action<Exception>? error, Action? onCompled)
        {
            var observer = new SingleTimeObserver<TEvent>(handler)
                           {
                               Error = error,
                               OnCompled = onCompled
                           };

            return observer.Register(observable);
        }

        public static IDisposable SingleTimeSubscribe<TEvent>(this IObservable<TEvent> observable, Action<TEvent> handler, Action<Exception>? error)
            => SingleTimeSubscribe(observable, handler, error, null);

        public static IDisposable SingleTimeSubscribe<TEvent>(this IObservable<TEvent> observable, Action<TEvent> handler)
            => SingleTimeSubscribe(observable, handler, null);

        public static IObservable<IActorRef> NotNobody(this IObservable<IActorRef> observable) 
            => observable.Where(a => !a.IsNobody());
    }
}