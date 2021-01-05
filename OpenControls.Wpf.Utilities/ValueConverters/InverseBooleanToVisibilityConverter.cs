using System;
using System.Windows;
using System.Windows.Data;

namespace OpenControls.Wpf.Utilities.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                throw new NotSupportedException();
            }

            Type valueType = (value == null) ? null : value.GetType();

            if (valueType != null)
            {
                if ((valueType != typeof(bool?)) && (valueType != typeof(bool)))
                {
                    throw new NotSupportedException();
                }
            }

            bool flag = false;

            if (valueType == typeof(bool))
            {
                flag = (bool)value;
            }
            else if (valueType == typeof(bool?))
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }

            return (flag ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
