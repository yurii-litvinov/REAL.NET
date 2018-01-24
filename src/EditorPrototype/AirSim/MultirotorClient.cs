namespace EditorPrototype
{
    using System;
    using System.Runtime.InteropServices;

    public class MutirotorClient : IDisposable
    {
        private IntPtr client;

        public void CreateClient() => this.client = CreateClientCPP();

        public void ConfirmConnection() => ConfirmConnectionCPP(this.client);

        public void EnableApiControl() => EnableApiControlCPP(this.client);

        public void ArmDisarm(bool isArm) => ArmDisarmCPP(this.client, isArm);

        public void Takeoff(float timeout) => TakeoffCPP(this.client, timeout);

        public void Sleep(float time) => SleepClientCPP(this.client, time);

        public void Hover() => HoverCPP(this.client);

        public void MoveByVelocityZ(float speed) => MoveByVelocityZCPP(this.client, speed);

        public void Land() => LandCPP(this.client);

        public void Dispose()
        {
            Land();
            DisposeClientCPP(this.client);
        }

        [DllImport("HelloDrone.dll")]
        private static extern IntPtr CreateClientCPP();

        [DllImport("HelloDrone.dll")]
        private static extern void DisposeClientCPP(IntPtr ptr);

        [DllImport("HelloDrone.dll")]
        private static extern void ConfirmConnectionCPP(IntPtr ptr);

        [DllImport("HelloDrone.dll")]
        private static extern void ArmDisarmCPP(IntPtr ptr, bool isArm);

        [DllImport("HelloDrone.dll")]
        private static extern void TakeoffCPP(IntPtr ptr, float timeout);

        [DllImport("HelloDrone.dll")]
        private static extern void HoverCPP(IntPtr ptr);

        [DllImport("HelloDrone.dll")]
        private static extern void SleepClientCPP(IntPtr ptr, float time);

        [DllImport("HelloDrone.dll")]
        private static extern void LandCPP(IntPtr ptr);

        [DllImport("HelloDrone.dll")]
        private static extern void MoveByVelocityZCPP(IntPtr ptr, float speed);

        [DllImport("HelloDrone.dll")]
        private static extern void EnableApiControlCPP(IntPtr ptr);

    }
}