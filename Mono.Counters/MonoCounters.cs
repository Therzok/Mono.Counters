using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Mono.Counters
{
    public sealed class MonoCounters
    {
        static readonly bool IsSupported = Type.GetType("Mono.Runtime") != null;

        readonly Native.CountersEnumCallback cb;
        readonly int bufferSize;

        public MonoCounters() : this(8192)
        {
        }

        public MonoCounters(int bufferSize)
        {
            if (!IsSupported)
                throw new PlatformNotSupportedException("Only works on Mono");

            this.bufferSize = bufferSize;

            cb = new Native.CountersEnumCallback(CountersForeachCallback);
        }

        public void Dump(CounterVisitor visitor)
        {
            using var data = new CallbackData(visitor, bufferSize);
            Native.mono_counters_foreach(cb, data.Handle);
        }

        // TODO: Need to multi-target to support different mono runtime flavours
        //[MonoPInvokeCallback(typeof(Native.CountersEnumCallback))]
        static int CountersForeachCallback(IntPtr counterPtr, IntPtr user_data)
        {
            var data = (CallbackData)((GCHandle)user_data).Target;
            data.Process(new NativeCounter(counterPtr));

            return 1;
        }

        sealed class CallbackData : IDisposable
        {
            readonly CounterVisitor visitor;
            byte[]? array;
            GCHandle gch;

            public CallbackData(CounterVisitor visitor, int bufferSize)
            {
                this.visitor = visitor;

                array = ArrayPool<byte>.Shared.Rent(bufferSize);
                gch = GCHandle.Alloc(this);
            }

            public void Process(NativeCounter counter)
            {
                if (visitor.Filter(counter))
                    visitor.Process(new Counter(counter.Name, counter.GetValue(array!)));
            }

            public IntPtr Handle => (IntPtr)gch;

            public void Dispose()
            {
                if (array != null)
                {
                    ArrayPool<byte>.Shared.Return(array);
                    array = null;
                }

                if (gch.IsAllocated)
                {
                    gch.Free();
                }
            }
        }
    }
}
