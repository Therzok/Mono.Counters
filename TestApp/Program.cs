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
                Console.Write("{0}: {1}", c.Name, c.Value);

                if (c.Value is TimeSpan ts)
                {
                    Console.Write(" [{0}ms]", ts.TotalMilliseconds);
                }

                Console.WriteLine();
            }
        }

        sealed class TypeVisitor : CounterVisitor
        {
            readonly CounterSection mask;

            public TypeVisitor() : this(CounterSection.Mask)
            {
            }

            public TypeVisitor(CounterSection mask)
            {
                this.mask = mask;
            }

            protected sealed override bool Filter(NativeCounter c)
            {
                if ((c.Section & mask) != 0) {
                    Console.Write("{0,10} {1,10} {2,10} ", c.Section, c.Type, c.Unit);
                    return true;
                }

                return false;
            }

            protected sealed override void Process(Counter c)
            {
                Console.WriteLine(c.Name);
            }
        }

        [DllImport("__Internal")]
        static extern void mono_counters_dump(int section_mask, IntPtr file);

        [DllImport("__Internal")]
        static extern void mono_counters_enable(int section_mask);

        public static void Main(string[] args)
        {
            // There's something weird about console redirecting here, so please use:
            // `mono --stats TestApp.exe` from a terminal.
            const int COUNT = 10000;

            var counters = new MonoCounters();
            var visitor = new Visitor();

            for (int i = 0; i < COUNT; ++i)
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
            Console.WriteLine("================");
            Console.WriteLine("COUNTER METADATA");
            Console.WriteLine("================");
            Console.WriteLine();

            counters.Dump(new TypeVisitor(CounterSection.Gc));

            mono_counters_enable(0);

            Console.WriteLine();
            Console.WriteLine("=============");
            Console.WriteLine("MONO SHUTDOWN");
            Console.WriteLine("=============");
            Console.WriteLine();
        }
    }
}
