using System;
using System.Threading.Tasks;
using Tauron.Application.Workshop.Mutating;

namespace Tauron.Application.Workshop.StateManagement
{
    public interface IReducer<TData>
    {
        IObservable<ReducerResult<TData>> Reduce(IObservable<MutatingContext<TData>> state, IStateAction action);

        bool ShouldReduceStateForAction(IStateAction action);
    }
}