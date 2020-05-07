using PerformersScene.Models;

namespace PerformersScene.RobotInterfaces
{
    public interface IRobotMaze
    {
        Side[,] VerticalLines { get; }
        
        int Width { get; }
        
        Side[,] HorizontalLines { get; }
        
        int Height { get; }
        
        Cell GetCell(IntPoint position);
    }
}