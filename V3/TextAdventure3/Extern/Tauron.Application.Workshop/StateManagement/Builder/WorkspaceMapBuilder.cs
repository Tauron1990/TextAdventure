﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Autofac;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement.Internal;

namespace Tauron.Application.Workshop.StateManagement.Builder
{
    public class WorkspaceMapBuilder<TData> : StateBuilderBase, IWorkspaceMapBuilder<TData>
        where TData : class
    {
        private readonly Func<WorkspaceBase<TData>> _workspace;
        private readonly Dictionary<Type, Func<WorkspaceBase<TData>, IStateAction, IDataMutation>> _map = new();

        public WorkspaceMapBuilder(Func<WorkspaceBase<TData>> workspace) 
            => _workspace = workspace;

        public override (StateContainer State, string Key) Materialize(MutatingEngine engine, IComponentContext? componentContext) 
            => (new WorkspaceContainer<TData>(_map.ToImmutableDictionary(), _workspace()), string.Empty);

        public IWorkspaceMapBuilder<TData> MapAction<TAction>(Func<WorkspaceBase<TData>, IDataMutation> to)
        {
            _map[typeof(TAction)] = (work, _) => to(work);
            return this;
        }

        public IWorkspaceMapBuilder<TData> MapAction<TAction>(Func<WorkspaceBase<TData>, TAction, IDataMutation> to)
        {
            _map[typeof(TAction)] = (work, action) => to(work, (TAction) action);
            return this;
        }
    }
}