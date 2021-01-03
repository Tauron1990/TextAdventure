using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextAdventure.Editor.Views
{
    public class MainContentView : UserControl
    {
        public MainContentView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
