using PerformersScene.RobotInterfaces;

namespace PerformersScene.Models.DataLayer
{
    public class RobotMaze : IRobotMaze
    {
        public RobotMaze(Side[,] horizontalLines, Side[,] verticalLines)
        {
            this.Height = horizontalLines.GetLength(0) - 1;
            this.HorizontalLines = horizontalLines;
            this.Width = verticalLines.GetLength(0) - 1;
            this.VerticalLines = verticalLines;
        }

        public Side[,] VerticalLines { get; }

        public int Width { get; }

        public Side[,] HorizontalLines { get; }

        public int Height { get; }
    }


}