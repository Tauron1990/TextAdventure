using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using StrongInject;
using TextAdventure.Editor.Views;

namespace TextAdventure.Editor
{
    public class App : Application
    {
        public static MainWindow MainWindow { get; private set; } = null!;

        public static IDispatcher Dispatcher { get; private set; } = null!;
        
        public override void Initialize() 
            => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher = Avalonia.Threading.Dispatcher.UIThread;
                
                var container = new RootContainer();
                var mainwindow = container.Resolve();

                desktop.Exit += (_, _) =>
                                {
                                    mainwindow.Dispose();
                                    container.Dispose();
                                };

                MainWindow = mainwindow.Value.View;
                desktop.MainWindow = MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}