using System;
using System.Windows;
using System.Windows.Data;

namespace OpenControls.Wpf.DockManager.Converters
{
    internal class CornerRadiusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((CornerRadius)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.CornerRadius))
            {
                throw new InvalidOperationException("The target must be a CornerRadius");
            }

            OpenControls.Wpf.DockManager.Controls.Utilities.Parse(value as string, out CornerRadius cornerRadius);

            return cornerRadius;
        }
    }
}
