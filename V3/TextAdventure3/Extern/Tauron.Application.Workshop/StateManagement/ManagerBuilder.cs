﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Functional.Maybe;
using JetBrains.Annotations;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement.Builder;
using Tauron.Application.Workshop.StateManagement.Dispatcher;

namespace Tauron.Application.Workshop.StateManagement
{
    [PublicAPI]
    public sealed class ManagerBuilder
    {
        public static RootManager CreateManager(WorkspaceSuperviser superviser, Action<ManagerBuilder> builder)
        {
            var managerBuilder = new ManagerBuilder(superviser);
            builder(managerBuilder);
            return managerBuilder.Build(Maybe<IComponentContext>.Nothing, Maybe<AutofacOptions>.Nothing);
        }

        public WorkspaceSuperviser Superviser { get; }

        private          Func<IStateDispatcherConfigurator> _dispatcherFunc = () => new DefaultStateDispatcher();
        private readonly List<Func<Maybe<IEffect>>>         _effects        = new();
        private readonly List<Func<Maybe<IMiddleware>>>     _middlewares    = new();
        private readonly List<StateBuilderBase>             _states         = new();

        private bool _sendBackSetting;

        internal ManagerBuilder(WorkspaceSuperviser superviser) 
            => Superviser = superviser;

        public IWorkspaceMapBuilder<TData> WithWorkspace<TData>(Func<WorkspaceBase<TData>> source)
            where TData : class
        {
            var builder = new WorkspaceMapBuilder<TData>(source);
            _states.Add(builder);
            return builder;
        }

        public IStateBuilder<TData> WithDataSource<TData>(Func<IExtendedDataSource<TData>> source) 
            where TData : class
        {
            var builder = new StateBuilder<TData>(source);
            _states.Add(builder);
            return builder;
        }

        public ManagerBuilder WithDefaultSendback(bool flag)
        {
            _sendBackSetting = flag;
            return this;
        }

        public ManagerBuilder WithMiddleware(Func<Maybe<IMiddleware>> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }

        public ManagerBuilder WithEffect(Func<Maybe<IEffect>> effect)
        {
            _effects.Add(effect);
            return this;
        }

        public ManagerBuilder WithDispatcher(Func<IStateDispatcherConfigurator> factory)
        {
            _dispatcherFunc = factory;
            return this;
        }

        internal RootManager Build(Maybe<IComponentContext> mayComponentContext, Maybe<AutofacOptions> mayAutofacOptions)
        {
            List<IEffect> additionalEffects = new();
            List<IMiddleware> additionalMiddlewares = new();

            var componets =
                from componentContext in mayComponentContext
                from options in mayAutofacOptions.Or(AutofacOptions.Default)
                select (componentContext.Resolve<IEnumerable<IEffect>>(), componentContext.Resolve<IEnumerable<IMiddleware>>());

            componets.Do(add =>
            {
                var (addEffects, addMiddlewares) = add;

                additionalEffects.AddRange(addEffects);
                additionalMiddlewares.AddRange(addMiddlewares);
            });

            var middlewares =
                from middleware in _middlewares
                let inst = middleware().OrElseDefault()
                where inst != null
                select inst;

            var effects =
                from effect in _effects
                let inst = effect().OrElseDefault()
                where inst != null
                select inst;
            
            return new RootManager(Superviser, _dispatcherFunc(), _states, effects.Concat(additionalEffects), 
                middlewares.Concat(additionalMiddlewares), _sendBackSetting, mayComponentContext);
        }
    }
}