using Logo.TurtleInterfaces;
using System;

namespace LogoScene.Models.DataLayer
{
    internal class TurtleCommander : ITurtleCommanderAsync
    {
        public ITurtle Turtle => turtle;

        public bool IsInProgress { get => inProgress; private set => inProgress = value; }

        private readonly Turtle turtle;

        private DoublePoint positionAfterMovement;

        private volatile bool inProgress;

        public TurtleCommander(Turtle turtle)
        {
            this.turtle = turtle;
            this.positionAfterMovement = new DoublePoint(0, 0);
            this.MovementPerformed += RaiseActionPerformed;
            this.RotationPerformed += RaiseActionPerformed;
            this.PenActionPerformed += RaiseActionPerformed;
            this.SpeedUpdatedPerformed += RaiseActionPerformed;
        }

        public TurtleCommander()
            : this(new Turtle(100, 100, 90, true))
        { }

        public event EventHandler<EventArgs> ActionPerformed;

        public event EventHandler<EventArgs> PenActionPerformed;

        public event EventHandler<EventArgs> MovementPerformed;

        public event EventHandler<EventArgs> RotationPerformed;

        public event EventHandler<EventArgs> SpeedUpdatedPerformed;

        public event EventHandler<LineEventArgs> MovementStarted;

        public event EventHandler<RotationEventArgs> RotationStarted;

        public event EventHandler<PenActionEventArgs> PenActionStarted;

        public event EventHandler<SpeedUpdateEventArgs> SpeedUpdateStarted;

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
            inProgress = true;
            PenActionStarted?.Invoke(this, new PenActionEventArgs());
        }

        public void PenUp()
        {
            turtle.SetIsPenDown(false);
            inProgress = true;
            PenActionStarted?.Invoke(this, new PenActionEventArgs());
        }

        public void RotateLeft(double degrees)
        {
            if (degrees < 0)
            {
                throw new ArgumentException("degrees should be non-negative");
            }
            turtle.SetAngle(turtle.Angle + degrees);
            inProgress = true;
            RotationStarted?.Invoke(this, new RotationEventArgs());
        }

        public void RotateRight(double degrees)
        {
            if (degrees < 0)
            {
                throw new ArgumentException("degrees should be non-negative");
            }
            turtle.SetAngle(turtle.Angle - degrees);
            inProgress = true;
            RotationStarted?.Invoke(this, new RotationEventArgs());
        }

        public void SetSpeed(double speed)
        {
            if (speed < 0)
            {
                throw new ArgumentException("speed should be non-negative");
            }
            turtle.SetSpeed(speed);
            IsInProgress = true;
            SpeedUpdateStarted?.Invoke(this, new SpeedUpdateEventArgs());
        }

        public void ResetTurtle()
        {
            IsInProgress = false;
            turtle.SetX(100);
            turtle.SetY(100);
            positionAfterMovement = new DoublePoint(100, 100);
            turtle.SetAngle(90);
            turtle.SetIsPenDown(true);
        }

        public void NotifyMovementPerformed()
        {
            lock (this)
            {
                turtle.SetX(positionAfterMovement.X);
                turtle.SetY(positionAfterMovement.Y);
                inProgress = false;
                MovementPerformed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void NotifyRotationPerformed()
        {
            lock (this)
            {
                inProgress = false;
                RotationPerformed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void NotifySpeedUpdatePerformed()
        {
            lock (this)
            {
                inProgress = false;
                SpeedUpdatedPerformed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void NotifyPenActionPerformed()
        {
            lock (this)
            {
                inProgress = false;
                PenActionPerformed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Move(double distance)
        {
            var oldX = turtle.X;
            var oldY = turtle.Y;
            var angleInRadians = turtle.Angle * Math.PI / 180;
            var newX = oldX + Math.Cos(angleInRadians) * distance;
            var newY = oldY + Math.Sin(angleInRadians) * distance;
            var oldPosition = new DoublePoint(oldX, oldY);
            positionAfterMovement = new DoublePoint(newX, newY);
            inProgress = true;
            MovementStarted?.Invoke(this, new LineEventArgs(oldPosition, positionAfterMovement));
        }

        public void Stop() => inProgress = false;

        private void RaiseActionPerformed(object sender, EventArgs e) => ActionPerformed?.Invoke(this, EventArgs.Empty);
    }
}
