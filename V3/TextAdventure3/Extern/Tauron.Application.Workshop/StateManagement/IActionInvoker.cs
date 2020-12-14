﻿using Functional.Maybe;
using JetBrains.Annotations;

namespace Tauron.Application.Workshop.StateManagement
{
    [PublicAPI]
    public interface IActionInvoker
    {
        Maybe<TState> GetState<TState>()
            where TState : class;

        Maybe<TState> GetState<TState>(Maybe<string> key)
            where TState : class;

        void Run(IStateAction action, bool? sendBack = null);
    }
}