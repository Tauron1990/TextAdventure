using System.Windows;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class WpfWindow : IWindow
    {
        private readonly Window _window;

        public WpfWindow(Window window) => _window = window;

        public void Show() => _window.Show();
        public void Hide() => _window.Hide();
        public bool? ShowDialog() => _window.ShowDialog();
    }
}