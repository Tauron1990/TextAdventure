using System.Windows;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class WpfElement : IUIElement
    {
        private readonly FrameworkElement _element;

        public WpfElement(FrameworkElement element) => _element = element;
    }
}