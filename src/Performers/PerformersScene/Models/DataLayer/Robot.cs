using PerformersScene.RobotInterfaces;

namespace PerformersScene.Models.DataLayer
{
    public class Robot : IRobot
    {
        public static IRobot CreateRobot(Direction robotDirection, IntPoint position) => new Robot(robotDirection, position);

        private Robot(Direction robotDirection, IntPoint position)
        {
            Direction = robotDirection;
            Position = position;
        }

        public Direction Direction { get; }

        public IntPoint Position { get; }
        
        public IRobot NewDirection(Direction direction) => CreateRobot(direction, this.Position);

        public IRobot NewPosition(IntPoint position) => CreateRobot(Direction, position);
    }
}