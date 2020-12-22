using Avalonia.Controls;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Avalonia.AppCore
{
    public sealed class AvaloniaElement : IUIElement
    {
        private readonly Control _element;

        public AvaloniaElement(Control element) => _element = element;
    }
}