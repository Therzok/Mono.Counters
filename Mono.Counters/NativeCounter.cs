using System;
using System.Runtime.InteropServices;

namespace Mono.Counters
{
    public readonly ref struct NativeCounter
    {
        internal readonly IntPtr Handle;

        internal NativeCounter(IntPtr handle)
        {
            Handle = handle;
        }

        public CounterSection Section => Native.mono_counter_get_section(Handle);
        public CounterType Type => Native.mono_counter_get_type(Handle);
        public CounterUnit Unit => Native.mono_counter_get_unit(Handle);
        public CounterVariance Variance => Native.mono_counter_get_variance(Handle);

        internal string Name => Marshal.PtrToStringAuto(Native.mono_counter_get_name(Handle));

        internal object GetValue(byte[] array)
        {
            int size = Native.mono_counters_sample(Handle, array, array.Length);

            if (size > 0)
            {
                switch (Type)
                {
                    case CounterType.Int32:
                        return MemoryMarshal.Read<int>(array);
                    case CounterType.UInt32:
                        return MemoryMarshal.Read<uint>(array);
                    case CounterType.IntPtr:
                        return MemoryMarshal.Read<IntPtr>(array);
                    case CounterType.Long:
                        return MemoryMarshal.Read<long>(array);
                    case CounterType.ULong:
                        return MemoryMarshal.Read<ulong>(array);
                    case CounterType.Double:
                        return MemoryMarshal.Read<double>(array);
                    case CounterType.String:
                        unsafe
                        {
                            fixed (byte *p = array)
                            {
                                return Marshal.PtrToStringAuto((IntPtr)p, size);
                            }
                        }
                    case CounterType.TimeInterval:
                        var ts = MemoryMarshal.Read<long>(array);
                        return TimeSpan.FromMilliseconds(ts / 1000);
                }
            }

            throw new InvalidOperationException($"Buffer {array.Length} too small");
        }
    }
}
