using Logo.TurtleManipulation;


namespace LogoScene.Operations
{
    public static class PointOperations
    {
        public static DoublePoint Plus(DoublePoint pointA, DoublePoint pointB) => new DoublePoint(pointA.X + pointB.X, pointA.Y + pointB.Y);

        public static DoublePoint Minus(DoublePoint pointA, DoublePoint pointB) => new DoublePoint(pointA.X - pointB.X, pointA.Y - pointB.Y);

        public static DoublePoint ScalarMultiply(double scalar, DoublePoint point) => new DoublePoint(scalar * point.X, scalar * point.Y);
    }
}
