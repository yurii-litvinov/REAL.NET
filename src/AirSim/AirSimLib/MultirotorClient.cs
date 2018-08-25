/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace AirSim.AirSimLib
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// DLL library wrapper
    /// </summary>
    public class MultirotorClient : IDisposable
    {
        private IntPtr client;

        private const string LibName = "DroneLib.dll";
        private static readonly string currentPath = Environment.CurrentDirectory;

        public void CreateClient()
        {
            var substring = currentPath.Substring(0, currentPath.IndexOf(@"\bin", StringComparison.Ordinal)) +
                            @"\AirSimLib\" + LibName;
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

        public int GetImage()
        {
            int[] ar = new int[20];
            var t = GetImageCPP(this.client, ar);
            return 0;
        }

        public double GetDistanceByImage()
        {
            float[] ar = new float[40000];
            var t = GetDistanceImageCPP(this.client, ar);
            int h = (int)ar[t + 1];
            int w = (int)ar[t];
            int[] bar = new int[t];
            for (int i = 0; i < t; ++i)
            {
                if (ar[i] >= 1)
                    bar[i] = 255;
                else
                    bar[i] = (int)(ar[i] * 256);
            }
            Bitmap bitmap = new Bitmap(w, h);
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    Color newColor = Color.FromArgb(bar[i + (j * w)], bar[i + (j * w)], bar[i + (j * w)]);

                    bitmap.SetPixel(i, j, newColor);
                }
            }

            bitmap.Save("bbb.png");
            return bar[w / 2 + (h * w) / 2];
        }

        public double GetDistance()
            => GetDistanceCPP(this.client);

        public (float x, float y, float z) GetPos()
        {
            float[] pos = new float[3];
            GetPosCPP(this.client, pos);
            return (pos[0], pos[1], pos[2]);
        }

        public void Dispose()
        {
            this.Land();
            DisposeClientCPP(this.client);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(LibName)]
        private static extern IntPtr CreateClientCPP();

        [DllImport(LibName)]
        private static extern void DisposeClientCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern int GetDistanceImageCPP(IntPtr ptr, float[] ar);

        [DllImport(LibName)]
        private static extern int GetImageCPP(IntPtr ptr, int[] ar);

        [DllImport(LibName)]
        private static extern float GetDistanceCPP(IntPtr ptr);

        [DllImport(LibName)]
        private static extern void GetPosCPP(IntPtr ptr, float[] ar);

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