using System;
using System.Windows;
using System.Windows.Threading;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.Wpf.AppCore;
using Tauron.Application.Wpf.Dialogs;
using ShutdownMode = Tauron.Application.CommonUI.AppCore.ShutdownMode;

namespace Tauron.Application.Wpf
{
    public sealed class WpfFramework : CommonUIFramework
    {
        private sealed class InternalDispatcher : IUIDispatcher
        {
            private readonly Dispatcher _dispatcher;

            public InternalDispatcher(Dispatcher dispatcher) 
                => _dispatcher = dispatcher;

            public void Post(Action action) => _dispatcher.InvokeAsync(action);
        }
        
        private sealed class InternalApplication : IUIApplication
        {
            private readonly System.Windows.Application _application;
            
            public InternalApplication()
            {
                _application = new System.Windows.Application();
                Dispatcher = new InternalDispatcher(_application.Dispatcher);
                _application.Startup += (_, _) => OnStartup();
            }


            public event EventHandler? Startup;
            
            public ShutdownMode ShutdownMode
            {
                get => _application.ShutdownMode switch
                {
                    System.Windows.ShutdownMode.OnLastWindowClose => ShutdownMode.OnLastWindowClose,
                    System.Windows.ShutdownMode.OnMainWindowClose => ShutdownMode.OnMainWindowClose,
                    System.Windows.ShutdownMode.OnExplicitShutdown => ShutdownMode.OnExplicitShutdown,
                    _ => throw new ArgumentOutOfRangeException()
                };
                set
                {
                    switch (value)
                    {
                        case ShutdownMode.OnLastWindowClose:
                            _application.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
                            break;
                        case ShutdownMode.OnMainWindowClose:
                            _application.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
                            break;
                        case ShutdownMode.OnExplicitShutdown:
                            _application.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }
            }

            public IUIDispatcher Dispatcher { get; }

            public void Shutdown(int returnValue) => _application.Shutdown(returnValue);
            
            public int Run() => _application.Run();
            private void OnStartup() => Startup?.Invoke(this, EventArgs.Empty);
        }
        
        public override IUIApplication CreateDefault() => new InternalApplication();

        public override IWindow CreateMessageDialog(string title, string message)
        {
            var window = new Window();
            window.Content = new MessageDialog(title, message, b => window.DialogResult = b, true) { Margin = new Thickness(10) };

            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.ShowInTaskbar = false;

            return new WpfWindow(window);
        }

        public override object CreateDefaultMessageContent(string title, string message, Action<bool?>? result, bool canCnacel)
            => new MessageDialog(title, message, result, canCnacel);
    }
}