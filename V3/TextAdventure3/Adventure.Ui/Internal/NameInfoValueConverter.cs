using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Adventure.Ui.Internal
{
    public sealed class NameInfoValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NameInfo nameInfo)
            {
                return nameInfo switch
                {
                    NameInfo.Error => Brushes.Red,
                    NameInfo.Warning => Brushes.Yellow,
                    NameInfo.Ok => Brushes.Green,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
                throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}