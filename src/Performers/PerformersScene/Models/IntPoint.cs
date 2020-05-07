namespace PerformersScene.Models
{
    public readonly struct IntPoint
    {
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public static IntPoint operator +(IntPoint a, IntPoint b)
        {
            return new IntPoint(a.X + b.X, a.Y + b.Y);
        }

        public static IntPoint operator *(int k, IntPoint a)
        {
            return  new IntPoint(k * a.X, k * a.Y);
        }
    }
}