using System;
namespace Mono.Counters
{
    public abstract class CounterVisitor
    {
        internal protected virtual bool Filter(NativeCounter c)
        {
            return true;
        }

        internal protected abstract void Process(Counter c);
    }
}
