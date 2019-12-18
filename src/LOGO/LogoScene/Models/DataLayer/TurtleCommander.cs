using LogoScene.Models.DataLayer;
using System;
using System.Windows;

namespace LogoScene.Models.DataLayer
{
    internal class TurtleCommander : ITurtleCommander
    {
        public ITurtle Turtle => turtle;

        public bool InProgress { get => inProgress; private set => inProgress = value; }

        private readonly Turtle turtle;

        private Point positionAfterMovement;

        private volatile bool inProgress;

        public TurtleCommander(Turtle turtle)
        {
            this.turtle = turtle;
            this.positionAfterMovement = new Point();
            this.MovementPerformed += RaiseActionPerformed;
            this.RotationPerformed += RaiseActionPerformed;
            this.PenActionPerformed += RaiseActionPerformed;
        }

        public TurtleCommander()
            : this(new Turtle(100, 100, 90, true))
        { }

        public event EventHandler<EventArgs> ActionPerformed;

        public event EventHandler<EventArgs> PenActionPerformed;

        public event EventHandler<EventArgs> MovementPerformed;

        public event EventHandler<EventArgs> RotationPerformed;

        public event EventHandler<MovementEventArgs> MovementStarted;

        public void MoveBackward(double distance)
        {
            if (distance < 0)
            {
                throw new ArgumentException("distance should be non-negative");
            }
            Move(-distance);
        }

        public void MoveForward(double distance)
        {
            if (distance < 0)
            {
                throw new ArgumentException("distance should be non-negative");
            }
            Move(distance);
        }

        public void PenDown()
        {
            turtle.SetIsPenDown(true);
            PenActionPerformed?.Invoke(this, EventArgs.Empty);
        }

        public void PenUp()
        {
            turtle.SetIsPenDown(false);
            PenActionPerformed?.Invoke(this, EventArgs.Empty);
        }

        public void RotateLeft(double degrees)
        {
            if (degrees < 0)
            {
                throw new ArgumentException("degrees should be non-negative");
            }
            turtle.SetAngle(turtle.Angle + degrees);
            RotationPerformed?.Invoke(this, EventArgs.Empty);
        }

        public void RotateRight(double degrees)
        {
            if (degrees < 0)
            {
                throw new ArgumentException("degrees should be non-negative");
            }
            turtle.SetAngle(turtle.Angle - degrees);
            RotationPerformed?.Invoke(this, EventArgs.Empty);
        }

        public void SetSpeed(double speed)
        {
            if (speed < 0)
            {
                throw new ArgumentException("speed should be non-negative");
            }
            turtle.SetSpeed(speed);
        }

        public void NotifyMovementPerformed()
        {
            turtle.SetX(positionAfterMovement.X);
            turtle.SetY(positionAfterMovement.Y);
            inProgress = false;
            MovementPerformed?.Invoke(this, EventArgs.Empty);
        }

        private void Move(double distance)
        {
            var oldX = turtle.X;
            var oldY = turtle.Y;
            var angleInRadians = turtle.Angle * Math.PI / 180;
            var newX = oldX + Math.Cos(angleInRadians) * distance;
            var newY = oldY + Math.Sin(angleInRadians) * distance;
            var oldPosition = new Point(oldX, oldY);
            positionAfterMovement = new Point(newX, newY);
            inProgress = true;
            MovementStarted?.Invoke(this, new MovementEventArgs(oldPosition, positionAfterMovement));
        }

        private void RaiseActionPerformed(object sender, EventArgs e) => ActionPerformed?.Invoke(this, EventArgs.Empty);
    }
}
