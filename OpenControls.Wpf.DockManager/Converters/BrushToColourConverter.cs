using System;
using System.Windows.Media;
using System.Windows.Data;

namespace OpenControls.Wpf.DockManager.Converters
{
    internal class BrushToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Media.Brush))
            {
                throw new InvalidOperationException("The target must be a Brush");
            }

            return new SolidColorBrush((Color)value);
        }
    }
}
