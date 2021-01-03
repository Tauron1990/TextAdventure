using System.Threading.Tasks;
using Avalonia;
using Tauron.Host;
using TextAdventure.Editor.Views;

namespace TextAdventure.Editor
{
    public static class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static async Task Main(string[] args)
        {
            await ActorApplication.Create(args)
                                  .UseAvalonia<MainWindow, App>(BuildAvaloniaApp)
                                  .Build()
                                  .Run();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp(AppBuilder builder)
            => builder
              .UsePlatformDetect()
              .LogToTrace();
    }
}
