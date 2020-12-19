using System;
using JetBrains.Annotations;

namespace Tauron.Application.CommonUI.AppCore
{
    [PublicAPI]
    public enum ShutdownMode
    {
        OnLastWindowClose,
        OnMainWindowClose,
        OnExplicitShutdown
    }

    [PublicAPI]
    public interface IUIApplication
    {
        event EventHandler? Startup;

        ShutdownMode ShutdownMode { get; set; }
        
        IUIDispatcher Dispatcher { get; }
        
        void Shutdown(int returnValue);
        int Run();
    }

    [PublicAPI]
    public interface IUIDispatcher
    {
        void Invoke(Action action);
    }
}