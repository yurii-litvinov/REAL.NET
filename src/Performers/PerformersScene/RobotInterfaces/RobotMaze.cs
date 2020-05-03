using PerformersScene.Models;
using PerformersScene.RobotInterfaces;

namespace RobotInterfaces
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


        public Cell GetCell(IntPoint position)
        {
            var hasLeft = VerticalLines[position.X, position.Y].IsWall;
            var hasDown = HorizontalLines[position.X, position.Y].IsWall;
            var hasRight = VerticalLines[position.X + 1, position.Y].IsWall;
            var hasUp = HorizontalLines[position.X, position.Y + 1].IsWall;
            return new Cell(hasLeft, hasRight, hasUp, hasDown);
        }
    }


}