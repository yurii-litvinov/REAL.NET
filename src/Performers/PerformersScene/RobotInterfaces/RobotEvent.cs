using System;

namespace PerformersScene.RobotInterfaces
{
    public class RobotEvent : EventArgs
    {
        public readonly IRobot RobotData;

        public RobotEvent(IRobot robotData)
        {
            RobotData = robotData;
        }
    }
}