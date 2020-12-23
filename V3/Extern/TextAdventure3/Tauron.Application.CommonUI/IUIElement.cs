using System;

namespace Tauron.Application.CommonUI
{
    public interface IUIElement : IUIObject
    {
        object DataContext { get; set; }
        event Action<object> DataContextChanged;
        event Action Loaded;
        event Action Unloaded;
    }
}