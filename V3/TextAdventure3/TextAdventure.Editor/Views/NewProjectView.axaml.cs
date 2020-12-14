using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using ReactiveUI;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor.Views
{
    [UsedImplicitly]
    public class NewProjectView : ReactiveUserControl<NewProjectViewModel>
    {
        private Button NextButton => this.FindControl<Button>("NextButton");
        private Button CancelButton => this.FindControl<Button>("CancelButton");


        public NewProjectView()
        {
            InitializeComponent();

            this.WhenActivated(cd =>
                               {
                                   this.BindCommand(ViewModel, m => m.NextCommand, m => m.NextButton).DisposeWith(cd);
                                   this.BindCommand(ViewModel, m => m.CancelCommand, m => m.CancelButton).DisposeWith(cd);
                               });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
