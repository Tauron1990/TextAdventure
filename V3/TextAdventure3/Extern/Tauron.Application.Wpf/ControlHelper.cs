﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Akka.Actor;
using JetBrains.Annotations;
using Serilog;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.Helper;
using Tauron.Application.CommonUI.ModelMessages;
using Tauron.Application.Wpf.AppCore;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
    public static class ControlHelper
    {
        private const string ControlHelperPrefix = "ControlHelper.";

        public static readonly DependencyProperty MarkControlProperty =
            DependencyProperty.RegisterAttached("MarkControl", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(string.Empty, MarkControl));

        public static readonly DependencyProperty MarkWindowProperty =
            DependencyProperty.RegisterAttached("MarkWindow", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(null, MarkWindowChanged));

        public static string GetMarkControl(DependencyObject obj)
        {
            return (string) Argument.NotNull(obj, nameof(obj)).GetValue(MarkControlProperty);
        }

        public static string GetMarkWindow(DependencyObject obj)
        {
            return (string) Argument.NotNull(obj, nameof(obj)).GetValue(MarkWindowProperty);
        }

        public static void SetMarkControl(DependencyObject obj, string value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(MarkControlProperty, Argument.NotNull(value, nameof(value)));
        }

        public static void SetMarkWindow(DependencyObject obj, string value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(MarkWindowProperty, Argument.NotNull(value, nameof(value)));
        }

        private static void MarkControl(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLinker(d, e.OldValue as string, e.NewValue as string, () => new ControlLinker());
        }

        private static void MarkWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLinker(d, e.OldValue as string, e.NewValue as string, () => new WindowLinker());
        }

        private static void SetLinker(DependencyObject obj, string? oldName, string? newName, Func<LinkerBase> factory)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return;

            Argument.NotNull(obj, nameof(obj));
            Argument.NotNull(factory, nameof(factory));
            if (DesignerProperties.GetIsInDesignMode(obj)) return;

            var ele = ElementMapper.Create(obj);
            var root = ControlBindLogic.FindRoot(ele);
            if (root == null)
            {
                ControlBindLogic.MakeLazy((IUIElement) ele, newName, oldName, 
                    (name, old, controllable, dependencyObject) => SetLinker(old, name, controllable, dependencyObject, factory));
                return;
            }

            SetLinker(newName, oldName, root, ele, factory);
        }

        private static void SetLinker(string? newName, string? oldName, IBinderControllable root, IUIObject obj, Func<LinkerBase> factory)
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

                var priTarget = ((WpfObject)AffectedObject).DependencyObject;

                if (windowName == null)
                {
                    if (!(priTarget is System.Windows.Window))
                        priTarget = System.Windows.Window.GetWindow(priTarget);

                    if (priTarget == null)
                        Log.Logger.Error($"ControlHelper: No Window Found: {DataContext.GetType()}|{realName}");
                }
                else
                {
                    priTarget =
                        System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().FirstOrDefault(win => win.Name == windowName);

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