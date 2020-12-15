﻿using System;
using System.Collections.Immutable;
using System.Reactive;
using Tauron.Application.Workshop.Mutation;

namespace Tauron.Application.Workshop.StateManagement.Internal
{
    public sealed class WorkspaceContainer<TData> : StateContainer
        where TData : class
    {
        private readonly ImmutableDictionary<Type, Func<WorkspaceBase<TData>, IStateAction, IDataMutation>> _map;
        private readonly WorkspaceBase<TData> _source;

        public WorkspaceContainer(ImmutableDictionary<Type, Func<WorkspaceBase<TData>, IStateAction, IDataMutation>> map, WorkspaceBase<TData> source)
            : base(source)
        {
            _map = map;
            _source = source;
        }

        public override IDataMutation? TryDipatch(IStateAction action, IObserver<IReducerResult> sendResult, IObserver<Unit> onCompled)
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
            private readonly Func<IDataMutation> _run;
            private readonly IObserver<IReducerResult> _result;
            private readonly IObserver<Unit> _compled;

            public WorkspaceMutation(Func<IDataMutation> run, IObserver<IReducerResult> result, IObserver<Unit> compled, object hashKey, string actionName)
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
                    switch (_run())
                    {
                        case ISyncMutation mut:
                            mut.Run();
                            break;
                        case IAsyncMutation mut:
                            mut.Run().Wait(TimeSpan.FromMinutes(1));
                            break;
                        default:
                            throw new InvalidOperationException("Unkowen Data Mutation Type");
                    }
                }
                catch (Exception e)
                {
                    _result.OnNext(new ErrorResult(e));
                    throw;
                }
                finally
                {
                    _compled.OnNext(Unit.Default);
                    _compled.OnCompleted();
                    _result.OnCompleted();
                }
            }

            private sealed class ErrorResult : IReducerResult
            {
                public bool IsOk => false;

                public string[]? Errors { get; }

                public ErrorResult(Exception e) 
                    => Errors = new[] {e.Message};
            }
        }
    }
}