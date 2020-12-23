﻿using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Akka.Util;
using JetBrains.Annotations;
using Tauron.Akka;

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
        void Post(Action action);
        Task InvokeAsync(Action action);

        Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> action);

        Task<TResult> InvokeAsync<TResult>(Func<TResult> action);
    }
    
    public sealed class DispatcherScheduler : LocalScheduler
    {
        private readonly IUIDispatcher _dispatcher;

        private DispatcherScheduler(IUIDispatcher dispatcher) => _dispatcher = dispatcher;

        public static IScheduler From(IUIDispatcher dispatcher)
            => new DispatcherScheduler(dispatcher);
        
        public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var target = Scheduler.Normalize(dueTime);

            SingleAssignmentDisposable disposable = new();

            void TryRun()
            {
                if (disposable.IsDisposed) return;
                disposable.Disposable = action(this, state);
            }

            if (target == TimeSpan.Zero)
                _dispatcher.Post(TryRun);
            else
            {
                var timerDispose = new SingleAssignmentDisposable();
                Timer timer = new(o =>
                                  {
                                      _dispatcher.Post(TryRun);
                                      ((IDisposable)o!).Dispose();
                                  }, timerDispose, dueTime, Timeout.InfiniteTimeSpan);

                timerDispose.Disposable = timer;
            }

            return disposable;
        }
    }
}