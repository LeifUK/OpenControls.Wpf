using System;
using System.Windows.Data;

namespace OpenControls.Wpf.Utilities.ValueConverters
{
    [ValueConversion(typeof(object), typeof(Boolean))]
    public class ObjectToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Boolean))
            {
                throw new NotSupportedException();
            }

            return (value != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
