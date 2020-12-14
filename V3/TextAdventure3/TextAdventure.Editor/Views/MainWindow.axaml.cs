using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using StrongInject;
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
    
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private MenuItem OpenProject => this.FindControl<MenuItem>("OpenProject");
        private MenuItem CloseProject => this.FindControl<MenuItem>("CloseProject");

        private MenuItem NewProject => this.FindControl<MenuItem>("NewProject");


        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(cd =>
                               {
                                   this.BindCommand(ViewModel, m => m.OpenCommand, m => m.OpenProject).DisposeWith(cd);
                                   this.BindCommand(ViewModel, m => m.CloseProjectCommand, m => m.CloseProject).DisposeWith(cd);
                                   this.BindCommand(ViewModel, m => m.NewProjectCommand, w => w.NewProject).DisposeWith(cd);
                               });
        }

        private void InitializeComponent() 
            => AvaloniaXamlLoader.Load(this);
    }
}