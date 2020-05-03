using System.Drawing;
using System.Windows;
using PerformersScene.Models;

namespace RobotInterfaces
{
    public class Robot : IRobot
    {
        public static IRobot CreateRobot(Direction robotDirection, IntPoint position) => new Robot(robotDirection, position);

        private Robot(Direction robotDirection, IntPoint position)
        {
            RobotDirection = robotDirection;
            Position = position;
        }

        public Direction RobotDirection { get; }

        public IntPoint Position { get; }
        
        public IRobot NewDirection(Direction direction) => CreateRobot(direction, this.Position);

        public IRobot NewPosition(IntPoint position) => CreateRobot(RobotDirection, position);
    }
}