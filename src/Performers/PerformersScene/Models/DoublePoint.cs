namespace PerformersScene.Models
{
    public readonly struct DoublePoint
    {
        public DoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }
    }
}
