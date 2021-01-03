﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using Tauron.Application.CommonUI.Model;

namespace Tauron.Application.CommonUI
{
    [PublicAPI]
    public static class ActorFlowWpfExetension
    {
        #region Command

        public static CommandRegistrationBuilder ThenFlow(this CommandRegistrationBuilder builder, Func<IObservable<Unit>, IDisposable> flowBuilder) 
            => ThenFlow(builder, Unit.Default, flowBuilder);

        public static CommandRegistrationBuilder ThenFlow<TStart>(this CommandRegistrationBuilder builder, TStart trigger, Func<IObservable<TStart>, IDisposable> flowBuilder) 
            => ThenFlow(builder, () => trigger, flowBuilder);

        public static CommandRegistrationBuilder ThenFlow<TStart>(this CommandRegistrationBuilder builder, Func<TStart> trigger, Func<IObservable<TStart>, IDisposable> flowBuilder)
        {
            var ob = new Subject<TStart>();
            var sub = flowBuilder(ob.AsObservable());

            builder.Target.AddResource(ob);
            builder.Target.AddResource(sub);

            return builder.WithExecute(() => ob.OnNext(trigger()));
        }

        public static IObservable<Unit> ToModel<TRecieve>(this IObservable<TRecieve> selector, IViewModel model)
            => selector.ToActor(model.Actor);

        #endregion

        #region UIProperty

        public static FluentPropertyRegistration<TData> ThenFlow<TData>(this FluentPropertyRegistration<TData> prop, Func<IObservable<TData>, IDisposable> flowBuilder)
        {
            var aFlow = new Subject<TData>();
            
            prop.Actor.AddResource(flowBuilder(aFlow.AsObservable()));
            prop.Actor.AddResource(aFlow);

            return prop.OnChange(d => aFlow.OnNext(d));
        }

        #endregion
    }
}
