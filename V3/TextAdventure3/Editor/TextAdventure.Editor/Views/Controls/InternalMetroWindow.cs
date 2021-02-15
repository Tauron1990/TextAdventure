using System;
using System.Reactive;
using System.Windows;
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Helper;
using Tauron.Application.CommonUI.UI;
using Tauron.Application.Wpf.AppCore;

namespace TextAdventure.Editor.Views.Controls
{
    public sealed class InternalMaterialWindowLogic : ControlLogicBase<InternalMaterialWindow>
    {
        public InternalMaterialWindowLogic([NotNull] InternalMaterialWindow userControl, [NotNull] IViewModel model) :
            base(userControl, model)
        {
            userControl.SizeToContent = SizeToContent.Manual;
            userControl.ShowInTaskbar = true;
            userControl.ResizeMode = ResizeMode.CanResize;
            userControl.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        protected override void WireUpUnloaded()
        {
            UserControl.Closed += (_, _) => UserControlOnUnloaded();
        }
    }

    public class InternalMaterialWindow : MaterialWindow, IView, IUIElement, IWindowProvider
    {
        private readonly InternalMaterialWindowLogic _controlLogic;
        private readonly IWindow _element;

        protected InternalMaterialWindow(IViewModel viewModel)
        {
            _element = new WpfWindow(this);
            _controlLogic = new InternalMaterialWindowLogic(this, viewModel);
        }

        IUIObject? IUIObject.GetPerent() => _element.GetPerent();
        public object Object => this;

        IObservable<object> IUIElement.DataContextChanged => _element.DataContextChanged;

        IObservable<Unit> IUIElement.Loaded => _element.Loaded;

        IObservable<Unit> IUIElement.Unloaded => _element.Unloaded;

        public void Register(string key, IControlBindable bindable, IUIObject affectedPart)
        {
            _controlLogic.Register(key, bindable, affectedPart);
        }

        public void CleanUp(string key)
        {
            _controlLogic.CleanUp(key);
        }

        public event Action? ControlUnload
        {
            add => _controlLogic.ControlUnload += value;
            remove => _controlLogic.ControlUnload -= value;
        }

        IWindow IWindowProvider.Window => _element;
    }
}