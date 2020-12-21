using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Tauron.Application.Avalonia.Dialogs
{
    public class DialogHost : TemplatedControl
    {
        public DialogHost()
        {
            WindowBase
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
