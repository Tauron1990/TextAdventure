using System;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using StrongInject;
using Tauron.Application.Avalonia;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor.Views
{
    [Register(typeof(NewProjectViewModel))]
    [Register(typeof(MainContentViewModel))]
    [Register(typeof(ErrorDisplayViewModel))]
    [Register(typeof(MainWindowViewModel))]
    [Register(typeof(MainWindow))]
    [Register(typeof(ViewFactory<MainWindow, MainWindowViewModel>))]
    public class MainWindowModule { }
    
    public class MainWindow : Window, IMainWindow
    {
        public MainWindow(IViewModel<MainWindowViewModel> model)
            : base(model)
        {
            InitializeComponent();
            Closed += (sender, args) => Shutdown?.Invoke(sender, args);
        }

        private void InitializeComponent() 
            => AvaloniaXamlLoader.Load(this);

        public IWindow             Window => this;
        public event EventHandler? Shutdown;
    }
}