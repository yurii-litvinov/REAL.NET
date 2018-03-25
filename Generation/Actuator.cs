namespace Generation
{
    using System;
    
    public class Actuator
    {
        public int Num { get; }

        public Actuator(int num)
        {
            this.Num = num;
        }

        public void Action(int value)
        {
            if (value > 0)
            {
                Console.WriteLine("Actuator {0} has acted!", Num);
            }
        }
    }
}
