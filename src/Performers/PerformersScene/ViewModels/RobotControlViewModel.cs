using System;
using System.ComponentModel;
using System.Windows.Input;
using PerformersScene.Models;
using PerformersScene.Models.Constants;
using PerformersScene.Models.DataLayer;
using PerformersScene.Operations;
using PerformersScene.RobotInterfaces;
using PerformersScene.ViewModels.Animation;

namespace PerformersScene.ViewModels
{
    public class RobotControlViewModel : ViewModelBase, IAnimationCompletedHandler
    {
        private double angle = 90;

        private double speedRatio = 1;

        private DoublePoint start;

        private DoublePoint end;

        public event EventHandler<EventArgs> AnimationCompleted;

        public RobotControlViewModel()
        {
            var half = new DoublePoint(RobotConstants.MazeSideLength / 2, RobotConstants.MazeSideLength / 2);
            initial = PointOperations.Plus(half, new DoublePoint(100, 100));
            this.AnimationCompletedCommand = new RelayCommand(
                (parameter) =>
                {
                    isMovingStarted = false;
                    RaiseAnimationCompletedEvent();
                });
            Start = initial;
            End = initial;
            IsMovingStarted = false;
        }

        private void RaiseAnimationCompletedEvent()
        {
            IsMovingStarted = false;
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
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

        public double Angle
        {
            get => angle;
            set
            {
                angle = value;
                OnPropertyChanged();
            }
        }

        public double CenterX => RobotConstants.RobotWidth / 2;

        public double CenterY => RobotConstants.RobotHeight / 2;

        public DoublePoint Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged();
            }
        }

        public DoublePoint End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChanged();
            }
        }

        public double RobotHeight => RobotConstants.RobotHeight;

        public double RobotWidth => RobotConstants.RobotWidth;

        public bool IsMovingStarted
        {
            get => isMovingStarted;
            set
            {
                isMovingStarted = value;
                OnPropertyChanged();
            }
        }

        private readonly DoublePoint initial;

        private bool isMovingStarted;
        
        private RobotCommander robotCommander;

        public void MoveRobot(IntPoint newPosition)
        {
            var calc = new RobotPositionCalculator(RobotConstants.MazeSideLength, initial);
            this.Start = this.End;
            this.End = calc.GetPosition(newPosition);
            IsMovingStarted = true;
        }

        public void SetDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    Angle = 90;
                    break;
                case Direction.Down:
                    Angle = 270;
                    break;
                case Direction.Right:
                    Angle = 0;
                    break;
                case Direction.Left:
                    Angle = 180;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            AnimationCompletedCommand.Execute(null);
        }

        public ICommand AnimationCompletedCommand { get; set; }

        public DoublePoint Shift { get; } =
            new DoublePoint(RobotConstants.RobotWidth / 2, RobotConstants.RobotHeight / 2);
    }
}