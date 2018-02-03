namespace EditorPrototype
{
    using System;
    using System.Runtime.InteropServices;

    public class MultirotorClient : IDisposable
    {
        private IntPtr client;
        private string defaultDirectory;

        public void CreateClient()
        {
            defaultDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("bin", StringComparison.Ordinal));
            this.client = CreateClientCPP();
        }

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
            Environment.CurrentDirectory = defaultDirectory;
        }

        [DllImport("DroneLib.dll")]
        private static extern IntPtr CreateClientCPP();

        [DllImport("DroneLib.dll")]
        private static extern void DisposeClientCPP(IntPtr ptr);

        [DllImport("DroneLib.dll")]
        private static extern void ConfirmConnectionCPP(IntPtr ptr);

        [DllImport("DroneLib.dll")]
        private static extern void ArmDisarmCPP(IntPtr ptr, bool isArm);

        [DllImport("DroneLib.dll")]
        private static extern void TakeoffCPP(IntPtr ptr, float timeout);

        [DllImport("DroneLib.dll")]
        private static extern void HoverCPP(IntPtr ptr);

        [DllImport("DroneLib.dll")]
        private static extern void SleepClientCPP(IntPtr ptr, float time);

        [DllImport("DroneLib.dll")]
        private static extern void LandCPP(IntPtr ptr);

        [DllImport("DroneLib.dll")]
        private static extern void MoveByVelocityZCPP(IntPtr ptr, float speed);

        [DllImport("DroneLib.dll")]
        private static extern void EnableApiControlCPP(IntPtr ptr);

    }
}