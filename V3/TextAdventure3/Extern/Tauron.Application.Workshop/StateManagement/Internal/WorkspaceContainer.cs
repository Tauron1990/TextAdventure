﻿using System;
using System.Collections.Immutable;
using Functional.Maybe;
using Tauron.Application.Workshop.Mutation;

namespace Tauron.Application.Workshop.StateManagement.Internal
{
    public sealed class WorkspaceContainer<TData> : StateContainer
        where TData : class
    {
        private readonly ImmutableDictionary<Type, Func<WorkspaceBase<TData>, IStateAction, IDataMutation>> _map;
        private readonly WorkspaceBase<TData> _source;

        public WorkspaceContainer(ImmutableDictionary<Type, Func<WorkspaceBase<TData>, IStateAction, IDataMutation>> map, WorkspaceBase<TData> source)
            : base(((IState)source).ToMaybe())
        {
            _map = map;
            _source = source;
        }

        public override IDataMutation? TryDipatch(IStateAction action, Action<IReducerResult> sendResult, Action onCompled)
        {
            var type = action.GetType();
            return !_map.TryGetValue(type, out var runner) 
                ? null 
                : new WorkspaceMutation(() => runner(_source, action), sendResult, onCompled, action.ActionName, action.ActionName);
        }

        public override void Dispose()
        {

        }

        private sealed class WorkspaceMutation : ISyncMutation
        {
            private readonly Action _run;
            private readonly Action<IReducerResult> _result;
            private readonly Action _compled;

            public WorkspaceMutation(Action run, Action<IReducerResult> result, Action compled, object hashKey, string actionName)
            {
                _run = run;
                _result = result;
                _compled = compled;
                ConsistentHashKey = hashKey;
                Name = actionName;
            }

            public object ConsistentHashKey { get; }

            public string Name { get; }

            public Action Run => Execute;

            private void Execute()
            {
                try
                {
                    _run();
                }
                catch (Exception e)
                {
                    _result(ErrorResult.From(e));
                    throw;
                }
                finally
                {
                    _compled();
                }
            }

            private sealed record ErrorResult(Maybe<string[]> Errors) : IReducerResult
            {
                public bool IsOk => false;

                public static ErrorResult From(Exception e) => new (new[] {e.Message}.ToMaybe());
            }
        }
    }
}