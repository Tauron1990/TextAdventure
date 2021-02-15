﻿using System;
using System.Diagnostics;
using System.Linq;
using Akka.Actor;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JetBrains.Annotations;
using Serilog;
using Tauron.Application.Avalonia.AppCore;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.Helper;
using Tauron.Application.CommonUI.ModelMessages;

namespace Tauron.Application.Avalonia
{
    [PublicAPI]
    public class ControlHelper
    {
        private const string ControlHelperPrefix = "ControlHelper.";

        public static readonly AttachedProperty<string> MarkControlProperty =
            AvaloniaProperty.RegisterAttached<ControlHelper, Control, string>("MarkControl", string.Empty);

        public static readonly AttachedProperty<string> MarkWindowProperty =
            AvaloniaProperty.RegisterAttached<ControlHelper, Control, string>("MarkWindow", string.Empty);

        private ControlHelper()
        {
        }

        public static string GetMarkControl(Control obj)
            => Argument.NotNull(obj, nameof(obj)).GetValue(MarkControlProperty);

        public static string GetMarkWindow(Control obj)
            => Argument.NotNull(obj, nameof(obj)).GetValue(MarkWindowProperty);

        public static void SetMarkControl(Control obj, string value)
        {
            var old = GetMarkControl(obj);
            Argument.NotNull(obj, nameof(obj)).SetValue(MarkControlProperty, Argument.NotNull(value, nameof(value)));
            MarkControl(obj, value, old);
        }

        public static void SetMarkWindow(Control obj, string value)
        {
            var old = GetMarkWindow(obj);
            Argument.NotNull(obj, nameof(obj)).SetValue(MarkWindowProperty, Argument.NotNull(value, nameof(value)));
            MarkWindowChanged(obj, value, old);
        }

        private static void MarkControl(AvaloniaObject d, string newValue, string oldValue)
        {
            SetLinker(d, oldValue, newValue, () => new ControlLinker());
        }

        private static void MarkWindowChanged(AvaloniaObject d, string newValue, string oldValue)
        {
            SetLinker(d, oldValue, newValue, () => new WindowLinker());
        }

        private static void SetLinker(AvaloniaObject obj, string? oldName, string? newName, Func<LinkerBase> factory)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return;

            Argument.NotNull(obj, nameof(obj));
            Argument.NotNull(factory, nameof(factory));

            var ele = ElementMapper.Create(obj);
            var root = ControlBindLogic.FindRoot(ele);
            if (root == null)
            {
                ControlBindLogic.MakeLazy((IUIElement) ele, newName, oldName,
                    (name, old, controllable, dependencyObject)
                        => SetLinker(old, name, controllable, dependencyObject, factory));
                return;
            }

            SetLinker(newName, oldName, root, ele, factory);
        }

        private static void SetLinker(string? newName, string? oldName, IBinderControllable root, IUIObject obj,
            Func<LinkerBase> factory)
        {
            if (oldName != null)
                root.CleanUp(ControlHelperPrefix + oldName);

            if (newName == null) return;

            var linker = factory();
            linker.Name = newName;
            root.Register(ControlHelperPrefix + newName, linker, obj);
        }

        [DebuggerNonUserCode]
        private class ControlLinker : LinkerBase
        {
            protected override void Scan()
            {
                if (DataContext is IViewModel model && AffectedObject is IUIElement element)
                    model.Actor.Tell(new ControlSetEvent(Name, element));
            }
        }

        private abstract class LinkerBase : ControlBindableBase
        {
            public string Name { get; set; } = string.Empty;

            protected object DataContext { get; private set; } = new();

            protected abstract void Scan();

            protected override void CleanUp()
            {
            }

            protected override void Bind(object context)
            {
                DataContext = context;
                Scan();
            }
        }

        private class WindowLinker : LinkerBase
        {
            protected override void Scan()
            {
                var realName = Name;
                string? windowName = null;

                if (realName.Contains(":"))
                {
                    var nameSplit = realName.Split(new[] {':'}, 2);
                    realName = nameSplit[0];
                    windowName = nameSplit[1];
                }

                var priTarget = ((AvaObject) AffectedObject).Obj;

                if (windowName == null)
                {
                    if (!(priTarget is Window))
                        while (priTarget != null)
                            priTarget = priTarget is StyledElement {Parent: StyledElement parent} ? parent : null;

                    if (priTarget == null)
                        Log.Logger.Error($"ControlHelper: No Window Found: {DataContext.GetType()}|{realName}");
                }
                else
                {
                    priTarget =
                        global::Avalonia.Application.Current.ApplicationLifetime is
                            IClassicDesktopStyleApplicationLifetime lifetime
                            ? lifetime.Windows.FirstOrDefault(win => win.Name == windowName)
                            : null;

                    if (priTarget == null)
                        Log.Logger.Error($"ControlHelper: No Window Named {windowName} Found");
                }

                if (priTarget == null) return;

                if (DataContext is IViewModel model && ElementMapper.Create(priTarget) is IUIElement element)
                    model.Actor.Tell(new ControlSetEvent(Name, element));
            }
        }
    }
}