using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Dialogs;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        public MainWindow(IViewModel<MainWindowViewModel> model, IDialogCoordinator coordinator)
            : base(model)
        {
            InitializeComponent();

            var diag = (IDialogCoordinatorUIEvents) coordinator;

            diag.ShowDialogEvent += o => this.ShowDialog(o);
            diag.HideDialogEvent += () => ((ICommand) DialogHost.CloseDialogCommand).Execute(null);

            Closed += (sender, args) => Shutdown?.Invoke(sender, args);
        }

        public event EventHandler? Shutdown;
    }
}
