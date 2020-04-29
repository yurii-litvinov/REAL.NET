using System;
using System.Globalization;
using System.Windows.Data;

namespace LogoScene.Controls.Converters
{
    public class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var toNegative = (double)value;
            return -toNegative;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
