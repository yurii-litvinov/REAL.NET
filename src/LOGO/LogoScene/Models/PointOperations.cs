using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogoScene.Models.Operations
{
    public static class PointOperations
    { 
        public static Point Plus(Point pointA, Point pointB) => new Point(pointA.X + pointB.X, pointA.Y + pointB.Y);

        public static Point Minus(Point pointA, Point pointB) => new Point(pointA.X - pointB.X, pointA.Y - pointB.Y);

        public static Point ScalarMultiply(double scalar, Point point) => new Point(scalar * point.X, scalar * point.Y);
    }
}
