﻿using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Tauron.Application.CommonUI.UI
{
    [PublicAPI]
    public sealed class ViewModelConverterExtension : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new ViewModelConverter();
        }
    }

    public class ViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if(!(parameter is IView root)) return value;
            if (!(value is IViewModel model)) return value;

            //var view = root.ViewManager.Get(model, root);
            //if (view != null)
            //    return view;

            var manager = AutoViewLocation.Manager;
            var view = manager.ResolveView(model);
            return view ?? value;

            //root.ViewManager.ThenRegister(model, view, root);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FrameworkObject(value).DataContext;
        }
    }
}