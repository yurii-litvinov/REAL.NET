using PerformersScene.Models;

namespace PerformersScene.RobotInterfaces
{
    public interface IRobot
    {
        Direction Direction { get; }
        
        IntPoint Position { get; }

        IRobot NewDirection(Direction direction);

        IRobot NewPosition(IntPoint position);
    }
}