using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using PerformersScene.Models;
using PerformersScene.Models.DataLayer;
using RobotInterfaces;
using IRobotMaze = PerformersScene.RobotInterfaces.IRobotMaze;

namespace PerformersScene.ViewModels
{
    public class RobotSceneViewModel : ViewModelBase
    {
        public RobotSceneViewModel()
        {
            MazeLines.Add(new LineViewModel());
            maze = InitMaze();
            GenerateLines();
        }

        private IRobotMaze InitMaze()
        {
            var horizontal = new Side[,]
            {
                {new Side(true), new Side(true), new Side(true), new Side(true)},
                {new Side(false), new Side(true), new Side(false), new Side(false)},
                {new Side(false), new Side(false), new Side(false), new Side(false)},
                {new Side(true), new Side(true), new Side(true), new Side(true)}
            };
            var vertical = new Side[,]
            {
                {new Side(true), new Side(true), new Side(true)},
                {new Side(true), new Side(false), new Side(false)},
                {new Side(false), new Side(false), new Side(false)},
                {new Side(false), new Side(false), new Side(false)}, 
                {new Side(true), new Side(true), new Side(true)}
            };
            return new RobotMaze(horizontal, vertical);
        }

        public ObservableCollection<LineViewModel> MazeLines { get; } = new ObservableCollection<LineViewModel>();

        private RobotScene model;

        private readonly IRobotMaze maze;

        private readonly double lineLength = Constants.MazeSideLength;

        private static Brush GetColor(bool isWall) => isWall ? Brushes.Black : Brushes.Gray;

        private double GetThickness(bool isWall) => isWall ? 2 : 1;

        private void GenerateLines()
        {
            double initialX = 100;
            double initialY = 100;
            var width = maze.Width;
            var height = maze.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var horizontalLine = new LineViewModel
                    {
                        X1 = initialX + j * lineLength,
                        X2 = initialX + (j + 1) * lineLength,
                        Y1 = initialY + i * lineLength,
                        Y2 = initialY + i * lineLength,
                        LineColor = GetColor(maze.HorizontalLines[i, j].IsWall),
                        Thickness = GetThickness(maze.HorizontalLines[i, j].IsWall)
                    };
                    MazeLines.Add(horizontalLine);
                    var verticalLine = new LineViewModel
                    {
                        X1 = initialX + j * lineLength,
                        X2 = initialX + j * lineLength,
                        Y1 = initialY + i * lineLength,
                        Y2 = initialY + (i + 1) * lineLength,
                        LineColor = GetColor(maze.VerticalLines[j, i].IsWall),
                        Thickness = GetThickness(maze.VerticalLines[j, i].IsWall)
                    };
                    MazeLines.Add(verticalLine);
                }
            }

            for (int j = 0; j < width; j++)
            {
                var horizontalLine = new LineViewModel
                {
                    X1 = initialX + j * lineLength,
                    X2 = initialX + (j + 1) * lineLength,
                    Y1 = initialY + height * lineLength,
                    Y2 = initialY + height * lineLength,
                    LineColor = GetColor(maze.HorizontalLines[height, j].IsWall),
                    Thickness = GetThickness(maze.HorizontalLines[height, j].IsWall)
                };
                MazeLines.Add(horizontalLine);   
            }

            for (int i = 0; i < height; i++)
            {
                var verticalLine = new LineViewModel
                {
                    X1 = initialX + width * lineLength,
                    X2 = initialX + width * lineLength,
                    Y1 = initialY + i * lineLength,
                    Y2 = initialY + (i + 1) * lineLength,
                    LineColor = GetColor(maze.VerticalLines[width, i].IsWall),
                    Thickness = GetThickness(maze.VerticalLines[width, i].IsWall),
                };
                MazeLines.Add(verticalLine);   
            }
        }
    }
}