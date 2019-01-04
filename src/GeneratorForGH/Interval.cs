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
    /// Open interval (Min, Max). Helps to represent intervals from model in repo.
    /// </summary>
    /// Interval gets int as the input and if this value falls within a given interval (Min, Max)
    /// then it publishes the event with positive number (Num + 1) -- so as not to fall into zero,
    /// otherwise -- the event with negative number -(Num + 1).
    public class Interval
    {
        public int Num { get; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        public event EventHandler<int> Event;

        public Interval(int num)
        {
            this.Num = num;
        }

        public void Action(int value)
        {
            if ((!Min.HasValue || Min < value) &&
                (!Max.HasValue || Max > value))
            {
                //Console.WriteLine("Interval{0} ({1}, {2}) TRUE", Num, this.Min, this.Max);
                this.Event?.Invoke(this, this.Num + 1);
            }
            else
            {
                //Console.WriteLine("Interval{0} ({1}, {2}) FALSE", Num, this.Min, this.Max);
                this.Event?.Invoke(this, -(this.Num + 1));
            }
        }
    }
}

