using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WpfEditor.AirSim
{
    public class MultirotorClient : IDisposable
    {
        private IntPtr client;

        private const string LibName = "DroneLib.dll";
        static readonly string currentPath = Environment.CurrentDirectory;

        public void CreateClient()
        {
            var substring = currentPath.Substring(0, currentPath.IndexOf(@"\bin", StringComparison.Ordinal)) +
                            @"\Airsim\" + LibName;
            LoadLibrary(substring);
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
        }


        [DllImport("kernel32.dll")]

        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(LibName)]
        private static extern IntPtr CreateClientCPP();

        [DllImport(LibName)]
        private static extern void DisposeClientCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern void ConfirmConnectionCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern void ArmDisarmCPP(IntPtr ptr, bool isArm);

        [DllImport(LibName)]
        private static extern void TakeoffCPP(IntPtr ptr, float timeout);

        [DllImport(LibName)]
        private static extern void HoverCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern void SleepClientCPP(IntPtr ptr, float time);

        [DllImport(LibName)]
        private static extern void LandCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern void MoveByVelocityZCPP(IntPtr ptr, float speed);

        [DllImport(LibName)]
        private static extern void EnableApiControlCPP(IntPtr ptr);

    }
}