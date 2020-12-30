using System;
using System.Windows;
using System.Windows.Data;

namespace OpenControls.Wpf.DatabaseDialogs.View
{
    internal class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? false : true;
        }
    }
}
