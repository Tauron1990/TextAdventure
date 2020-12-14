﻿using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Workshop.StateManagement
{
    [PublicAPI]
    public interface IMiddleware
    {
        void Initialize(RootManager store);

        void AfterInitializeAllMiddlewares();

        bool MayDispatchAction(IStateAction action);

        void BeforeDispatch(IStateAction action);

    }
}