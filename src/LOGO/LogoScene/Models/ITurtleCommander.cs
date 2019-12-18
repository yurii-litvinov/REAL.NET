using LogoScene.Models.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.Models
{
    public interface ITurtleCommander
    {
        event EventHandler<MovementEventArgs> MovementStarted;

        event EventHandler<RotationEventArgs> RotationStarted;

        event EventHandler<PenActionEventArgs> PenActionStarted;

        event EventHandler<SpeedUpdateEventArgs> SpeedUpdateStarted;

        event EventHandler<EventArgs> ActionPerformed;

        event EventHandler<EventArgs> MovementPerformed;

        event EventHandler<EventArgs> RotationPerformed;

        event EventHandler<EventArgs> PenActionPerformed;

        event EventHandler<EventArgs> SpeedUpdatedPerformed;

        ITurtle Turtle { get; }

        bool InProgress { get; }

        void MoveForward(double distance);

        void MoveBackward(double distance);

        void RotateLeft(double degrees);

        void RotateRight(double degrees);

        void SetSpeed(double speed);

        void PenDown();

        void PenUp();
    }
}
