using System;
using PerformersScene.TurtleInterfaces;

namespace Logo.TurtleInterfaces
{
    public interface ITurtleCommanderAsync : ITurtleCommander
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

        bool IsInProgress { get; }

        void Stop();
    }
}
