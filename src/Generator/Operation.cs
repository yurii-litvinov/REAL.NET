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
using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// Logical operation class. Helps to represent operations from model in repo.
    /// Operation gets int as the input. Positive numbers are recognized as true, negative -- as false.
    /// </summary>
    public class Operation
    {
        public int Num { get; }

        public Dictionary<int, bool?> IncomingValues { get; set; }

        public Operation(int num)
        {
            this.Num = num;
            this.IncomingValues = new Dictionary<int, bool?>();
        }

        public string Kind { get; set; }

        public event EventHandler<int> Event;

        public void Action(int value)
        {
            int val;
            bool result;

            if (value < 0)
            {
                val = -value - 1;
                this.IncomingValues[val] = false;
                result = false;
            }
            else
            {
                val = value - 1;
                this.IncomingValues[val] = true;
                result = true;
            }

            if (this.Kind == "Or")
            {
                foreach (bool? v in this.IncomingValues.Values)
                {
                    if (v.HasValue)
                    {
                        result = result || v.Value;
                    }
                }
            }

            if (this.Kind == "And")
            {
                foreach (bool? v in this.IncomingValues.Values)
                {
                    if (v.HasValue)
                    {
                        result = result && v.Value;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }

            if (result)
            {
                //Console.WriteLine("Operation {0} TRUE", Num);
                this.Event(this, this.Num + 1);
            }
            else
            {
                //Console.WriteLine("Operation {0} FALSE", Num);
                this.Event(this, -(this.Num + 1));
            }
        }
    }
}

