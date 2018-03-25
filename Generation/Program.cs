namespace Generation
{
    using System;
	using System.Linq;
	using System.Reactive;
    using System.Reactive.Linq;

	public class GeneratedCode
    {
		private static Actuator element0;
		private static Operation element1;
		private static Interval element2;
		private static Interval element3;
		private static Sensor element4;
		private static Sensor element5;

		private static void Main(string[] args)
		{

			element0 = new Actuator(0);
	
			element1 = new Operation(1);
	
			element1.Kind = "And";

			element2 = new Interval(2);
	
			element2.Min = null;
			element2.Max = null;

			element3 = new Interval(3);
	
			element3.Min = null;
			element3.Max = null;

			element4 = new Sensor(4);
	
			element5 = new Sensor(5);
	

			element1.IncomingValues.Add(2, null);
	
			element1.IncomingValues.Add(3, null);
	// index of the element and index of the observer/observable which is made on it's basis are the same 

			var observer0 = Observer.Create<int>(x => element0.Action(x));
	
			var observer1 = Observer.Create<int>(x => element1.Action(x));
	
			IObservable<int> observable1 =
						Observable.FromEventPattern<EventHandler<int>, int>(
							h => element1.Event += h,
							h => element1.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize();

			var observer2 = Observer.Create<int>(x => element2.Action(x));
	
			IObservable<int> observable2 =
						Observable.FromEventPattern<EventHandler<int>, int>(
							h => element2.Event += h,
							h => element2.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize();

			var observer3 = Observer.Create<int>(x => element3.Action(x));
	
			IObservable<int> observable3 =
						Observable.FromEventPattern<EventHandler<int>, int>(
							h => element3.Event += h,
							h => element3.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize();

			IObservable<int> observable4 =
						Observable.FromEventPattern<EventHandler<int>, int>(
							h => element4.Event += h,
							h => element4.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize();

			IObservable<int> observable5 =
						Observable.FromEventPattern<EventHandler<int>, int>(
							h => element5.Event += h,
							h => element5.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize();
			var sub0 = observable1.Subscribe(observer0);
			var sub1 = observable2.Subscribe(observer1);
			var sub2 = observable4.Subscribe(observer2);
			var sub3 = observable3.Subscribe(observer1);
			var sub4 = observable5.Subscribe(observer3);

			ManualSimulator();
        }
		
		private static void ManualSimulator()
        {
            while (true)
            {

                Console.Write("4 -- for sensor4;    5 -- for sensor5: ");
                int value;
                int num;
                Sensor s = null;
                if (int.TryParse(Console.ReadLine(), out num))
                {
                    if (num == 4)
                    {
                        s = element4;
                    }
                    else
                    {
                        s = element5;
                    }

                    Console.Write("enter sensor value: ");
                    
                    if (s != null && int.TryParse(Console.ReadLine(), out value))
                    {
                        s.Action(value);
                    }
                }  
            }
        }
    }
}

