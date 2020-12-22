using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;

namespace Tauron.Application.Avalonia
{
    public sealed class AvaloniaFramework : CommonUIFramework
    {
        private sealed class AvaloniaDispatcher : IUIDispatcher
        {
            public void Post(Action action) => Dispatcher.UIThread.Post(action);
        }
        
        private sealed class AvalonApp : global::Avalonia.Application, IUIApplication
        {
            public override void OnFrameworkInitializationCompleted()
            {
                Startup?.Invoke(this, EventArgs.Empty);
                base.OnFrameworkInitializationCompleted();
            }

            public event EventHandler? Startup;
            
            public ShutdownMode ShutdownMode { get; set; }
            
            public IUIDispatcher Dispatcher { get; }
            
            public void Shutdown(int returnValue)
            {
                throw new NotImplementedException();
            }

            public int Run()
            {
                throw new NotImplementedException();
            }
        }
        
        public override IUIApplication CreateDefault()
        {
            throw new NotImplementedException();
        }

        public override IWindow CreateMessageDialog(string title, string message)
        {
            throw new NotImplementedException();
        }

        public override object CreateDefaultMessageContent(string title, string message, Action<bool?>? result, bool canCnacel)
        {
            throw new NotImplementedException();
        }
    }
}