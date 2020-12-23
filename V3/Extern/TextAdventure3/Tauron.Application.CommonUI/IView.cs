using System;
using Tauron.Application.CommonUI.Helper;
using Tauron.Application.CommonUI.UI;

namespace Tauron.Application.CommonUI
{
    public interface IView : IBinderControllable
    {
        string Key { get; }

        ViewManager ViewManager { get; }

        event Action? ControlUnload;
    }
}