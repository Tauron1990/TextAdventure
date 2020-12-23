﻿using System;
using JetBrains.Annotations;
using Tauron.Application.CommonUI.Helper;

namespace Tauron.Application.CommonUI.UI
{
    [PublicAPI]
    public class ActorBinding : BindingDecoratorBase
    {
        private readonly string _name;

        public ActorBinding(string name)
        {
            _name = name;
        }

        public override object? ProvideValue(IServiceProvider provider)
        {
            try
            {
                if (!TryGetTargetItems(provider, out var dependencyObject, out var prop))
                    return DependencyProperty.UnsetValue;

                if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                    return DependencyProperty.UnsetValue;

                if (!ControlBindLogic.FindDataContext(dependencyObject, out var model)) return null;

                Path = Path != null ? new PropertyPath("Value." + Path.Path, Path.PathParameters) : new PropertyPath("Value");
                Source = new DeferredSource(_name, model);
                Binding.Delay = 500;
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                Binding.ValidatesOnNotifyDataErrors = true;

                return base.ProvideValue(provider);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}