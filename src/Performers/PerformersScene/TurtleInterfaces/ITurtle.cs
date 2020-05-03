using Logo.TurtleInterfaces;
using DoublePoint = PerformersScene.Models.DoublePoint;

namespace PerformersScene.TurtleInterfaces
{
    public interface ITurtle
    {
        double Speed { get; }

        double X { get; }

        double Y { get; }

        DoublePoint Position { get; }

        double Angle { get; }

        bool IsPenDown { get; }
    }
}
