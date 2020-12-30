using System;
using System.Windows.Data;
using System.Globalization;

namespace OpenControls.Wpf.DatabaseDialogs.View
{
public class EnumComparisonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (parameter == null))
            {
                return false;
            }

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (parameter == null))
            {
                return false;
            }

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            return useValue ? Enum.Parse(targetType, targetValue) : null;
        }
    }
}
