﻿using System.Threading.Tasks;
using Tauron.Application.Workshop.Mutating;

namespace Tauron.Application.Workshop.StateManagement
{
    public interface IReducer<TData>
    {
        Task<ReducerResult<TData>> Reduce(MutatingContext<TData> state, IStateAction action);

        bool ShouldReduceStateForAction(IStateAction action);
    }
}