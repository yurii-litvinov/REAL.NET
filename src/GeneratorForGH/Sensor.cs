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

namespace GeneratorForGH
{
    using System;

    /// <summary>
    /// Helps to represent sensors from model in repo.
    /// </summary>
    public class Sensor
    {
        public int Num { get; }

        public Sensor(int num)
        {
            this.Num = num;
        }

        public int? Value { get; set; } = null;

        public event EventHandler<int> Event;

        public void Action(int value)
        {
            //Console.WriteLine("Sensor {0} : {1}", Num, value);
            this.Event?.Invoke(this, value);
        }
    }
}
