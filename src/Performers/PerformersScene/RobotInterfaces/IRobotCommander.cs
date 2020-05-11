namespace PerformersScene.RobotInterfaces
{
    public interface IRobotCommander
    {
        IRobot RobotPerformer { get; }

        void MoveForward();

        void MoveBackward();

        void RotateLeft();

        void RotateRight();

        void ResetRobot();
    }
}