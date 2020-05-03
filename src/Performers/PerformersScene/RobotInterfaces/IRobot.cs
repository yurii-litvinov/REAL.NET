using System.Windows;
using PerformersScene.Models;

namespace RobotInterfaces
{
    public interface IRobot
    {
        Direction RobotDirection { get; }
        
        IntPoint Position { get; }

        IRobot NewDirection(Direction direction);

        IRobot NewPosition(IntPoint position);
    }
}