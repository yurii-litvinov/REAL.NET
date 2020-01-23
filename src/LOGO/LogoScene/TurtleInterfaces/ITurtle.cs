namespace Logo.TurtleManipulation
{
    public interface ITurtle
    {
        double Speed { get; }

        double X { get; }

        double Y { get; }

        double Angle { get; }

        bool IsPenDown { get; }
    }
}
