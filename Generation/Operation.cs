namespace Generation
{
    using System;
    using System.Collections.Generic;

    //Operation gets int as the input.Positive numbers are recognized as true, negative -- as false.
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
                Console.WriteLine("Operation {0} TRUE", Num);
                this.Event(this, this.Num + 1);
            }
            else
            {
                Console.WriteLine("Operation {0} FALSE", Num);
                this.Event(this, -(this.Num + 1));
            }
        }
    }
}

