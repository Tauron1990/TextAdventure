using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.Threading;
using Tauron.Application.Avalonia.AppCore;
using Tauron.Application.Avalonia.Dialogs;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using ShutdownMode = Tauron.Application.CommonUI.AppCore.ShutdownMode;

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
            private ClassicDesktopStyleApplicationLifetime AppLifetime => (ClassicDesktopStyleApplicationLifetime)ApplicationLifetime;

            public AvalonApp() => Dispatcher = new AvaloniaDispatcher();
            

            public event EventHandler? Startup;

            public ShutdownMode ShutdownMode
            {
                get
                {
                    return AppLifetime.ShutdownMode switch
                    {
                        global::Avalonia.Controls.ShutdownMode.OnLastWindowClose => ShutdownMode.OnLastWindowClose,
                        global::Avalonia.Controls.ShutdownMode.OnMainWindowClose => ShutdownMode.OnMainWindowClose,
                        global::Avalonia.Controls.ShutdownMode.OnExplicitShutdown => ShutdownMode.OnExplicitShutdown,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
                set
                {
                    AppLifetime.ShutdownMode = value switch
                    {
                        ShutdownMode.OnLastWindowClose => global::Avalonia.Controls.ShutdownMode.OnLastWindowClose,
                        ShutdownMode.OnMainWindowClose => global::Avalonia.Controls.ShutdownMode.OnMainWindowClose,
                        ShutdownMode.OnExplicitShutdown => global::Avalonia.Controls.ShutdownMode.OnExplicitShutdown,
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                }
            }
            
            public IUIDispatcher Dispatcher { get; }
            
            public void Shutdown(int returnValue) 
                => Dispatcher.Post(() =>
                                   {
                                       AppLifetime.Shutdown(returnValue);
                                   });

            public int Run() 
                => AppLifetime.Start(Environment.GetCommandLineArgs());
        }

        public override IUIApplication CreateDefault()
            => (IUIApplication) AppBuilder
                               .Configure<AvalonApp>()
                               .UsePlatformDetect()
                               .LogToDebug()
                               .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime())
                               .Instance;
        
        public override IWindow CreateMessageDialog(string title, string message)
        {
            var window = new Window();
            window.Content = new MessageDialog(title, message, b => window.Close(b), true) { Margin = new Thickness(10) };
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Owner = ((IClassicDesktopStyleApplicationLifetime) global::Avalonia.Application.Current.ApplicationLifetime).MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowInTaskbar = false;

            return new AvaloniaWindow(window);
        }

        public override object CreateDefaultMessageContent(string title, string message, Action<bool?>? result, bool canCnacel)
            => new MessageDialog(title, message, result, canCnacel);
    }
}