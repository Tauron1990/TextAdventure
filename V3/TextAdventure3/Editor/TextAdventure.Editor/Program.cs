using System.Threading.Tasks;
using Tauron.Application.Workshop;
using Tauron.Host;

namespace TextAdventure.Editor
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await ActorApplication.Create(args)
                                  .UseWpf<MainWindow, App>()
                                  .AddModule<EditorModule>()
                                  .AddStateManagment(typeof(App).Assembly)
                                  .Build()
                                  .Run();
        }
    }
}