﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Autofac;
using CacheManager.Core;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement.Cache;
using Tauron.Application.Workshop.StateManagement.Internal;

namespace Tauron.Application.Workshop.StateManagement.Builder
{
    public abstract class StateBuilderBase
    {
        public abstract (StateContainer State, string Key) Materialize(MutatingEngine engine, ICache<object?>? parent, IComponentContext? componentContext);
    }

    public sealed class StateBuilder<TData> : StateBuilderBase, IStateBuilder<TData>
        where TData : class, IStateEntity
    {
        private readonly Func<IExtendedDataSource<TData>> _source;
        private readonly List<Func<IReducer<TData>>> _reducers = new List<Func<IReducer<TData>>>();

        private Type? _state;
        private bool _parentCache;
        private Action<ConfigurationBuilderCachePart>? _cacheConfigurator;
        private string? _key;

        public StateBuilder(Func<IExtendedDataSource<TData>> source)
            => _source = source;

        public IStateBuilder<TData> WithStateType<TState>()
            where TState : IState<TData>
        {
            _state = typeof(TState);
            return this;
        }

        public IStateBuilder<TData> WithStateType(Type type)
        {
            _state = type;
            return this;
        }

        public IStateBuilder<TData> WithNoCache()
        {
            _parentCache = false;
            _cacheConfigurator = null;
            return this;
        }

        public IStateBuilder<TData> WithParentCache()
        {
            _parentCache = true;
            return this;
        }

        public IStateBuilder<TData> WithCache(Action<ConfigurationBuilderCachePart> cache)
        {
            _cacheConfigurator = cache;
            return this;
        }

        public IStateBuilder<TData> WithReducer(Func<IReducer<TData>> reducer)
        {
            _reducers.Add(reducer);
            return this;
        }

        public IStateBuilder<TData> WithKey(string key)
        {
            _key = key;
            return this;
        }

        public override (StateContainer State, string Key) Materialize(MutatingEngine engine, ICache<object?>? parent, IComponentContext? componentContext)
        {
            if (_state == null)
                throw new InvalidOperationException("A State type or Instance Must be set");

            ICache<TData>? cache = null;
            if (_parentCache && parent != null || _cacheConfigurator != null)
            {
                cache = CacheFactory.Build<TData>(_key ?? Guid.NewGuid().ToString("N"), c =>
                {
                    if (_parentCache && parent != null)
                        c.WithParentCache(ParentCache.Create(parent, false, o => o as TData, data => data));
                    _cacheConfigurator?.Invoke(c);
                });
            }



            var cacheKey = $"{_state.Name}--{Guid.NewGuid():N}";
            var dataSource = new CachedDataSource<TData>(cacheKey, cache, _source());

            var dataEngine = MutatingEngine.From(dataSource, engine);

            IState? targetState = null;

            if (componentContext != null) 
                targetState = componentContext.ResolveOptional(_state, new TypedParameter(dataEngine.GetType(), dataEngine)) as IState;

            targetState ??= _state.FastCreateInstance(dataEngine) as IState;

            switch (targetState)
            {
                case ICanQuery<TData> canQuery:
                    canQuery.DataSource(dataSource);
                    break;
                case null:
                    throw new InvalidOperationException("Failed to Create State");
            }

            var container = new StateContainer<TData>(targetState, _reducers.Select(r => r()).ToImmutableList(), dataEngine, dataSource);

            return (container, _key ?? string.Empty);
        }
    }


}