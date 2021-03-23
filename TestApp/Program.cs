using System;
using System.Runtime.InteropServices;
using Mono.Counters;

namespace TestApp
{
    class MainClass
    {
        sealed class Visitor : CounterVisitor
        {
            protected sealed override bool Filter(NativeCounter c)
            {
                return c.Section == CounterSection.Gc;
            }

            protected sealed override void Process(Counter c)
            {
                Console.WriteLine("{0}: {1}", c.Name, c.Value);
            }
        }

        [DllImport("__Internal")]
        static extern void mono_counters_dump(int section_mask, IntPtr file);


        public static void Main(string[] args)
        {
            // There's something weird about console redirecting here, so please use:
            // `mono --stats TestApp.exe` from a terminal.

            var counters = new MonoCounters();
            var visitor = new Visitor();

            for (int i = 0; i < 10000; ++i)
            {
                _ = new string('a', 100);
                _ = new string('a', 100000);
            }

            Console.WriteLine();
            Console.WriteLine("=============");
            Console.WriteLine("MONO COUNTERS");
            Console.WriteLine("=============");
            Console.WriteLine();

            counters.Dump(visitor);

            Console.WriteLine();
            Console.WriteLine("=============");
            Console.WriteLine("MONO ORIGINAL");
            Console.WriteLine("=============");
            Console.WriteLine();

            mono_counters_dump((int)CounterSection.Gc, IntPtr.Zero);

            Console.WriteLine();
            Console.WriteLine("=============");
            Console.WriteLine("MONO SHUTDOWN");
            Console.WriteLine("=============");
            Console.WriteLine();
        }
    }
}
