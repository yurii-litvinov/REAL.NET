using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.TurtleInterfaces
{
    public interface ITurtleCommander
    {
        PerformersScene.TurtleInterfaces.ITurtle Turtle { get; }

        void MoveBackward(double distance);

        void MoveForward(double distance);

        void PenDown();

        void PenUp();

        void RotateLeft(double degrees);

        void RotateRight(double degrees);

        void SetSpeed(double speed);

        void ResetTurtle();
    }
}
