using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace Tauron.Akka
{
    [PublicAPI]
    public sealed record HookEvent(Delegate Invoker, Type Target, IDisposable? Disposable)
    {
        public static HookEvent Create<TType>(Action<TType> action)
            => new(action, typeof(TType), null);

        public static HookEvent Create<TType>(Func<IObservable<TType>, IDisposable> handler)
        {
            var subject = new Subject<TType>();
            var registration = new CompositeDisposable(subject, handler(subject.AsObservable()));

            return Create<TType>(m => subject.OnNext(m)) with{Disposable = registration};
        }
    }
}