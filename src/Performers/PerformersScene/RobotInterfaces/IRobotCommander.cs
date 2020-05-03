namespace RobotInterfaces
{
    public interface IRobotCommander
    {
        IRobot Robot { get; }

        void MoveForward();

        void MoveBackward();

        void Left();

        void Right();
    }
}