namespace Generation
{
    using System;

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
                Console.WriteLine("Interval {0} TRUE", Num);
                this.Event(this, this.Num + 1);
            }
            else
            {
                Console.WriteLine("Interval {0} FALSE", Num);
                this.Event(this, -(this.Num + 1));
            }
        }
    }
}

