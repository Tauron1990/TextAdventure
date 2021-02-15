﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Internal;
using JetBrains.Annotations;
using Tauron.Akka;

namespace Tauron
{
    [PublicAPI]
    public static class ObservableExtensions
    {
        public static IObservable<IActorRef> NotNobody(this IObservable<IActorRef> observable)
            => observable.Where(a => !a.IsNobody());

        public static IObservable<Unit> ToUnit<TSource>(this IObservable<TSource> input)
            => input.Select(_ => Unit.Default);

        public static IObservable<TType> Isonlate<TType>(this IObservable<TType> obs) => obs.Publish().RefCount();

        public static IObservable<Unit> ToUnit<TMessage>(this IObservable<TMessage> source, Action<TMessage> action)
        {
            return source.Select(m =>
                                 {
                                     action(m);
                                     return Unit.Default;
                                 });
        }

        public static IObservable<Unit> ToUnit<TMessage>(this IObservable<TMessage> source, Action action)
        {
            return source.Select(_ =>
                                 {
                                     action();
                                     return Unit.Default;
                                 });
        }

        public static IObservable<Unit> ToUnit<TMessage>(this IObservable<TMessage> source, Func<Task> action)
        {
            return source.SelectMany(async _ =>
                                     {
                                         await action();
                                         return Unit.Default;
                                     });
        }

        public static IDisposable MultiSubscribe<TType>(this IObservable<TType> obs,
            Action<IObservable<TType>, Action<IDisposable>> subs)
        {
            var dispo = new CompositeDisposable();

            subs(obs.Isonlate(), dispo.Add);

            return dispo;
        }

        public static IObservable<TValue> Lookup<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            => Observable.Defer(() => dictionary.TryGetValue(key, out var value)
                                    ? Observable.Return(value)
                                    : Observable.Empty<TValue>());

        #region Send To Actor

        public static IDisposable ToSelf<TMessage>(this IObservable<TMessage> obs)
            => ToActor(obs, ObservableActor.ExposedContext.Self);

        public static IDisposable ToParent<TMessage>(this IObservable<TMessage> source)
            => ToParent(source, ObservableActor.ExposedContext);

        public static IDisposable ToParent<TMessage>(this IObservable<TMessage> source, IActorContext context)
            => source.SubscribeWithStatus(m => context.Parent.Tell(m));

        public static IDisposable ToSender<TMessage>(this IObservable<TMessage> source)
            => ToSender(source, ObservableActor.ExposedContext);

        public static IDisposable ToSender<TMessage>(this IObservable<TMessage> source, IActorContext context)
            => source.SubscribeWithStatus(m => context.Sender.Tell(m));

        public static IDisposable ToActor<TMessage>(this IObservable<TMessage> source, IActorRef target)
            => source.SubscribeWithStatus(m => target.Tell(m));

        public static IDisposable ToActor<TMessage>(this IObservable<TMessage> source, Func<IActorRef> target)
            => source.SubscribeWithStatus(m => target().Tell(m));

        public static IDisposable ToActor<TMessage>(this IObservable<TMessage> source,
            Func<IActorContext, IActorRef> target)
            => source.SubscribeWithStatus(m => target(ObservableActor.ExposedContext).Tell(m));

        public static IDisposable ToActor<TMessage>(this IObservable<TMessage> source, Func<TMessage, IActorRef> target)
            => source.SubscribeWithStatus(m => target(m).Tell(m));


        public static IDisposable ForwardToParent<TMessage>(this IObservable<TMessage> source)
            => ForwardToParent(source, ObservableActor.ExposedContext);

        public static IDisposable ForwardToParent<TMessage>(this IObservable<TMessage> source, IActorContext context)
            => source.SubscribeWithStatus(m => context.Parent.Forward(m));

        public static IDisposable ForwardToSender<TMessage>(this IObservable<TMessage> source)
            => ForwardToSender(source, ObservableActor.ExposedContext);

        public static IDisposable ForwardToSender<TMessage>(this IObservable<TMessage> source, IActorContext context)
            => source.SubscribeWithStatus(m => context.Sender.Forward(m));

        public static IDisposable ForwardToActor<TMessage>(this IObservable<TMessage> source, IActorRef target)
            => source.SubscribeWithStatus(m => target.Forward(m));

        public static IDisposable ForwardToActor<TMessage>(this IObservable<TMessage> source, Func<IActorRef> target)
            => source.SubscribeWithStatus(m => target().Forward(m));

        public static IDisposable ForwardToActor<TMessage>(this IObservable<TMessage> source,
            Func<IActorContext, IActorRef> target)
            => source.SubscribeWithStatus(m => target(ObservableActor.ExposedContext).Forward(m));

        public static IDisposable ForwardToActor<TMessage>(this IObservable<TMessage> source,
            Func<TMessage, IActorRef> target)
            => source.SubscribeWithStatus(m => target(m).Forward(m));

        #endregion

        #region Subscriptions

        public static IDisposable SubscribeWithStatus<TMessage>(this IObservable<TMessage> source,
            object? sucessMessage, Action<TMessage> onNext)
        {
            var cell = InternalCurrentActorCellKeeper.Current;

            if (cell == null)
                return source.Subscribe(onNext);
            var self = cell.Self;
            return source.Subscribe(onNext, exception => self.Tell(new Status.Failure(exception)),
                () => self.Tell(new Status.Success(sucessMessage)));
        }

        public static IDisposable SubscribeWithStatus<TMessage>(this IObservable<TMessage> source,
            Action<TMessage> onNext)
            => SubscribeWithStatus(source, null, onNext);

        public static IDisposable SubscribeWithStatus<TMessage>(this IObservable<TMessage> source)
            => SubscribeWithStatus(source, null, _ => { });

        #endregion
    }
}