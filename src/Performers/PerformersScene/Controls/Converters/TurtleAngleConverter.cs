using System;
using System.Globalization;
using System.Windows.Data;

namespace PerformersScene.Controls.Converters
{
    public class TurtleAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var angle = (double)value;
            return -(angle - 90);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
