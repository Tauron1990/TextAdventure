using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tauron.Application.Avalonia.Dialogs
{
    public class MessageDialog : UserControl
    {
        public MessageDialog()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
