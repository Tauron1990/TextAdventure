using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor.Views
{
    [UsedImplicitly]
    public class ErrorDisplayView : ReactiveUserControl<ErrorDisplayViewModel>
    {
        public ErrorDisplayView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
