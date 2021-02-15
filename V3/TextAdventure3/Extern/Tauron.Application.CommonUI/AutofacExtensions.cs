﻿using System;
using Akka.Actor;
using Autofac;
using Autofac.Builder;
using JetBrains.Annotations;
using Tauron.Akka;
using Tauron.Application.CommonUI.Model;
using Tauron.Application.CommonUI.UI;

namespace Tauron.Application.CommonUI
{
    [PublicAPI]
    public static class AutofacExtensions
    {
        public static
            IRegistrationBuilder<ViewModelActorRef<TModel>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterView<TView, TModel>(this ContainerBuilder builder)
            where TView : IView where TModel : UiActor
        {
            AutoViewLocation.AddPair(typeof(TView), typeof(TModel));

            builder.RegisterType<TView>().As<TView>().InstancePerDependency();
            return builder.RegisterType<ViewModelActorRef<TModel>>().As<IViewModel<TModel>>()
                          .Keyed<IViewModel>(typeof(TModel)).InstancePerLifetimeScope()
                          .OnRelease(vm =>
                                     {
                                         if (vm.IsInitialized)
                                             vm.Actor.Tell(PoisonPill.Instance);
                                     });
        }

        public static
            IRegistrationBuilder<DefaultActorRef<TActor>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterModelActor<TActor>(this ContainerBuilder builder)
            where TActor : ActorModel
            => RegisterDefaultActor<TActor>(builder);

        public static
            IRegistrationBuilder<DefaultActorRef<TActor>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterDefaultActor<TActor>(this ContainerBuilder builder)
            where TActor : ActorBase
            => builder.RegisterType<DefaultActorRef<TActor>>().As<IDefaultActorRef<TActor>>();

        public static
            IRegistrationBuilder<SyncActorRef<TActor>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterSyncActor<TActor>(this ContainerBuilder builder)
            where TActor : ActorBase
            => builder.RegisterType<SyncActorRef<TActor>>().As<ISyncActorRef<TActor>>();

        public static IRegistrationBuilder<DefaultActorRef<TActor>, SimpleActivatorData, SingleRegistrationStyle>
            RegisterDefaultActor<TActor>(this ContainerBuilder builder,
                Func<IComponentContext, DefaultActorRef<TActor>> fac) where TActor : ActorBase
            => builder.Register(fac).As<IDefaultActorRef<TActor>>();

        public static IRegistrationBuilder<SyncActorRef<TActor>, SimpleActivatorData, SingleRegistrationStyle>
            RegisterSyncActor<TActor>(this ContainerBuilder builder,
                Func<IComponentContext, SyncActorRef<TActor>> fac) where TActor : ActorBase
            => builder.Register(fac).As<ISyncActorRef<TActor>>();
    }
}