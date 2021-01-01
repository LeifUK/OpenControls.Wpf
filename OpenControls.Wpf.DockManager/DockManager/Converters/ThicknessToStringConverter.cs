using System;
using System.Windows;
using System.Windows.Data;

namespace OpenControls.Wpf.DockManager.Converters
{
    class ThicknessToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Thickness)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Thickness))
            {
                throw new InvalidOperationException("The target must be a Thickness");
            }

            OpenControls.Wpf.DockManager.Controls.Utilities.Parse(value as string, out Thickness thickness);

            return thickness;
        }
    }
}
