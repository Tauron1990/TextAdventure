using System;
using System.ComponentModel;
using System.Windows.Data;
using Tauron.Application.CommonUI.Model;

namespace Tauron.Application.Wpf
{
    public static class WpfExtensions
    {
        public static FluentCollectionPropertyRegistration<TData> AndConfigurateView<TData>(this FluentCollectionPropertyRegistration<TData> collection, Action<ICollectionView> view)
        {
            view(CollectionViewSource.GetDefaultView(collection.Property.Value));
            return collection;
        }
    }
}