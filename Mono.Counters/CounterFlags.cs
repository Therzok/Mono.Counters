using System;

namespace Mono.Counters
{
    public enum CounterType : uint
    {
        /* Counter type, bits 0-7. */
        Int32,
        UInt32,
        IntPtr,
        Long,
        ULong,
        Double,
        String,
        TimeInterval, /* 64 bits signed int holding usecs. */
        Mask = 0xf,
        //Callback = 128, /* ORed with the other values */
    }

    [Flags]
    public enum CounterSection : uint
    {
        /* Sections, bits 8-23 (16 bits) */
        Mask = 0x00ffff00,
        Jit = 1 << 8,
        Gc = 1 << 9,
        Metadata = 1 << 10,
        Generics = 1 << 11,
        Security = 1 << 12,
        Runtime = 1 << 13,
        System = 1 << 14,
        PerfCounters = 1 << 15,
        Profiler = 1 << 16,
        Interp = 1 << 17,
        Tiered = 1 << 18,
    }

    public enum CounterUnit : uint
    {
        /* Unit, bits 24-27 (4 bits) */
        Mask = 0xFu << 24,
        Raw = 0 << 24,  /* Raw value */
        Bytes = 1 << 24, /* Quantity of bytes. RSS, active heap, etc */
        Time = 2 << 24,  /* Time interval in 100ns units. Minor pause, JIT compilation*/
        Count = 3 << 24, /*  Number of things (threads, queued jobs) or Number of events triggered (Major collections, Compiled methods).*/
        Percentage = 4 << 24, /* [0-1] Fraction Percentage of something. Load average. */
    }

    [Flags]
    public enum CounterVariance : uint
    {
        /* Monotonicity, bits 28-31 (4 bits) */
        Mask = 0xFu << 28, // MONO_COUNTER_VARIANCE_SHIFT,
        Monotonic = 1 << 28, /* This counter value always increase/decreases over time. Reported by --stat. */
        Constant = 1 << 29, /* Fixed value. Used by configuration data. */
        Variable = 1 << 30, /* This counter value can be anything on each sampling. Only interesting when sampling. */
    }
}
