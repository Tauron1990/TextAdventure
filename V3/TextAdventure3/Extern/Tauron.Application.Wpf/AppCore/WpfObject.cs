using System.Windows;
using System.Windows.Media;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Wpf.AppCore
{
    public class WpfObject : IUIObject
    {
        public WpfObject(DependencyObject obj) => DependencyObject = obj;

        public DependencyObject DependencyObject { get; }

        public IUIObject? GetPerent()
        {
            var temp = LogicalTreeHelper.GetParent(DependencyObject) ?? VisualTreeHelper.GetParent(DependencyObject);
            return temp == null ? null : ElementMapper.Create(temp);
        }
    }
}