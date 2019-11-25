using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LogoScene.Controls.Converters
{
    [ValueConversion(typeof(Point), typeof(Point))]
    public class TurtlePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var realPosition = (Point)value;
            return Minus(realPosition, halfTurtle);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transformedPosition = (Point)value;
            return Plus(transformedPosition, halfTurtle);
        }

        private Point halfTurtle = new Point(Models.Constants.TurtleWidth / 2, Models.Constants.TurtleHeight / 2);

        private Point Plus(Point pointA, Point pointB) => new Point(pointA.X + pointB.X, pointA.Y + pointB.Y);

        private Point Minus(Point pointA, Point pointB) => new Point(pointA.X - pointB.X, pointA.Y - pointB.Y);
    }
}
