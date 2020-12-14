using StrongInject;
using TextAdventure.Editor.ViewModels;
using TextAdventure.Editor.Views;

namespace TextAdventure.Editor
{
    [RegisterModule(typeof(MainWindowModule))]
    public sealed partial class RootContainer : IContainer<ViewFactory<MainWindow, MainWindowViewModel>>
    {
        
    }
}