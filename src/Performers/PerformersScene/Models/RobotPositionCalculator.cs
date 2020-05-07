using PerformersScene.Operations;

namespace PerformersScene.Models
{
    public class RobotPositionCalculator
    {
        private readonly double sideLength;
        
        private readonly DoublePoint initialPosition;

        public RobotPositionCalculator(double sideLength, DoublePoint initialPosition)
        {
            this.sideLength = sideLength;
            this.initialPosition = initialPosition;
        }

        public DoublePoint GetPosition(IntPoint coordinates)
        {
            var (x, y) = (coordinates.X, coordinates.Y);
            return PointOperations.Plus(new DoublePoint(x * sideLength, y * sideLength), initialPosition); 
        }
    }
}