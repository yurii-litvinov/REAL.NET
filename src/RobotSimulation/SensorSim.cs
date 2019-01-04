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

using System;
using System.Threading;

namespace RobotSimulation
{
    /// <summary>
    /// Simulates a real sensor generating random values at a random time moments
    /// </summary>
    public class SensorSim
    {
        private Timer timer;
        private Random rnd = new Random();

        public event EventHandler<int> Event;
        public int Index { get; set; }
        public int Port { get; set; }

        public SensorSim(int port)
        {
            int period = rnd.Next(0, 10);
            this.timer = new Timer(NewValue, null, period * 1000, period * 1000);
            this.Port = port;
        }

        private void NewValue(object o)
        {
            int period = rnd.Next(0, 10);
            int value = rnd.Next(30);
            var args = new SensorEventArgs
            {
                SensorIndex = this.Index,
                SensorValue = value
            };
            Console.WriteLine();
            Console.WriteLine("Sensor{0} at port {1} new value : {2}", this.Index, this.Port, value);

            this.Event?.Invoke(this, value);

            timer.Change(period * 1000, period * 1000);

        }

        public class SensorEventArgs : EventArgs
        {
            public int SensorIndex { get; set; }
            public int SensorValue { get; set; }
        }
    }
}
