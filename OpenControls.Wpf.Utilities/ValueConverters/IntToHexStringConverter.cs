using System;
using System.Windows.Data;

namespace OpenControls.Wpf.ValueUtilities.Converters
{
    internal class IntToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "0x" + ((int)value).ToString("X2"); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("IntToHexStringConverter.ConvertBack() - not implemented!");
        }
    }
}
