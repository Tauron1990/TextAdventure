using Tauron.Application.CommonUI;

namespace Tauron.Application.Avalonia.AppCore
{
    public sealed class AvaloniaWindow : IWindow
    {
        private readonly AvaloniaWindow _window;

        public AvaloniaWindow(AvaloniaWindow window) => _window = window;

        public void Show() => _window.Show();
        public void Hide() => _window.Hide();
        public bool? ShowDialog() => _window.ShowDialog();
    }
}