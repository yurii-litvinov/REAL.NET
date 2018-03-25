namespace Generation
{
    using System;

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
            Console.WriteLine("Sensor {0}", Num);
            this.Event(this, value);
        }
    }
}
