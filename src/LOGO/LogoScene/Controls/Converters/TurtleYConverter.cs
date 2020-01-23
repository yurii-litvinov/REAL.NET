using Logo.TurtleManipulation;
using LogoScene.Models;
using LogoScene.Operations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LogoScene.Controls.Converters
{
    public class TurtleYConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DoublePoint start = (DoublePoint)values[0];
            DoublePoint end = (DoublePoint)values[1];
            double speed = (double)values[2];
            double tag = (double)values[3];
            var lambda = tag;
            // position = (1-lambda)*start+lambda*end
            var position = PointOperations.Plus(PointOperations.ScalarMultiply(1 - lambda, start),
                PointOperations.ScalarMultiply(lambda, end));
            var shift = new DoublePoint(Constants.TurtleWidth / 2, Constants.TurtleHeight / 2);
            var transformedPosition = PointOperations.Minus(position, shift);
            return transformedPosition.Y;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
