using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using LogoScene.Models;
using LogoScene.Models.Operations;

namespace LogoScene.Controls.Converters
{
    public class TurtleXConverter : IMultiValueConverter
    {        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point start = (Point)values[0];
            Point end = (Point)values[1];
            double speed = (double)values[2];
            double tag = (double)values[3];
            var lambda = tag * speed;
            // position = (1-lambda)*start+lambda*end
            var position = PointOperations.Plus(PointOperations.ScalarMultiply(1 - lambda, start),
                PointOperations.ScalarMultiply(lambda, end));
            var shift = new Point(Constants.TurtleWidth / 2, Constants.TurtleHeight / 2);
            var transformedPosition = PointOperations.Minus(position, shift);
            return transformedPosition.X;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
