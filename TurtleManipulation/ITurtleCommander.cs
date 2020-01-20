using System;

namespace Logo.TurtleManipulation
{
    public interface ITurtleCommander
    {
        event EventHandler<LineEventArgs> MovementStarted;

        event EventHandler<RotationEventArgs> RotationStarted;

        event EventHandler<PenActionEventArgs> PenActionStarted;

        event EventHandler<SpeedUpdateEventArgs> SpeedUpdateStarted;

        event EventHandler<EventArgs> ActionPerformed;

        event EventHandler<EventArgs> MovementPerformed;

        event EventHandler<EventArgs> RotationPerformed;

        event EventHandler<EventArgs> PenActionPerformed;

        event EventHandler<EventArgs> SpeedUpdatedPerformed;

        ITurtle Turtle { get; }

        bool IsInProgress { get; }

        void MoveForward(double distance);

        void MoveBackward(double distance);

        void RotateLeft(double degrees);

        void RotateRight(double degrees);

        void SetSpeed(double speed);

        void PenDown();

        void PenUp();
    }
}
