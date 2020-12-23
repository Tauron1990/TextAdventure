using System.Threading.Tasks;
using Avalonia.Controls;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Avalonia.AppCore
{
    public sealed class AvaloniaWindow : IWindow
    {
        private readonly Window _window;

        public AvaloniaWindow(Window window) => _window = window;

        public void Show() => _window.Show();
        public void Hide() => _window.Hide();
        public Task<bool?> ShowDialog() => _window.ShowDialog<bool?>((Window) _window.Owner);
    }
}