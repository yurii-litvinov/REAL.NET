using System;

namespace PerformersScene.RobotInterfaces
{
    public interface IRobotCommanderAsync : IRobotCommander
    {
        event EventHandler<RobotEvent> MovementStarted;

        event EventHandler<RobotEvent> RotationStarted;

        event EventHandler<EventArgs> ActionDone;

        event EventHandler<EventArgs> RobotReset; 
        
        void Stop();
    }
}