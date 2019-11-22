using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogoScene.ViewModels
{
    public class DrawingSceneViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double TurtleWidth => turtleWidth;

        public double TurtleHeight => turtleHeight;

        public Point StartPointPath
        {
            get => Minus(startPoint, halfTurtle);
            set
            {
                startPoint = Plus(value, halfTurtle);
                OnPropertyChanged();
            }

        }

        public Point FinalPointPath
        {
            get => Minus(finalPoint, halfTurtle);
            set
            {
                finalPoint = Plus(value, halfTurtle);
                OnPropertyChanged();
            }
        }

        public double SpeedRatio
        {
            get => speedRatio;
            set
            {
                speedRatio = value;
                OnPropertyChanged();
            }
        }

        private double speedRatio = 0.3;

        private const double turtleWidth = 36;

        private const double turtleHeight = 50;

        private Point startPoint = new Point(100, 100);

        private Point finalPoint = new Point(100, 0);

        private Point halfTurtle = new Point(turtleWidth / 2, turtleHeight / 2);

        private void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private Point Plus(Point pointA, Point pointB) => new Point(pointA.X + pointB.X, pointA.Y + pointB.Y);

        private Point Minus(Point pointA, Point pointB) => new Point(pointA.X - pointB.X, pointA.Y - pointB.Y);
    }
}
