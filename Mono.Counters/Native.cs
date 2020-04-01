using System;
using System.Runtime.InteropServices;

namespace Mono.Counters
{
    static class Native
    {
        const string lib = "__Internal";

        #region registration

        [DllImport(lib)]
        public static extern void mono_counters_register(string descr, CounterType type, IntPtr addr);

        [DllImport(lib)]
        public static extern void mono_counters_register_with_size(string name, CounterType type, IntPtr addr, int size);

        public delegate void MonoCounterRegisterCallback(IntPtr ptr);

        [DllImport(lib)]
        public static extern void mono_counters_on_register(MonoCounterRegisterCallback callback);

        public delegate int CountersEnumCallback(IntPtr counter, IntPtr user_data);

        #endregion

        #region sampling

        [DllImport(lib)]
        public static extern void mono_counters_foreach(CountersEnumCallback cb, IntPtr user_data);

        [DllImport(lib)]
        public unsafe static extern int mono_counters_sample(IntPtr counter, byte[] buffer, int buffer_size);

        #endregion

        #region counter

        [DllImport(lib)]
        public static extern IntPtr mono_counter_get_name(IntPtr counter);

        [DllImport(lib)]
        public static extern CounterType mono_counter_get_type(IntPtr counter);

        [DllImport(lib)]
        public static extern CounterSection mono_counter_get_section(IntPtr counter);

        [DllImport(lib)]
        public static extern CounterUnit mono_counter_get_unit(IntPtr counter);

        [DllImport(lib)]
        public static extern CounterVariance mono_counter_get_variance(IntPtr counter);

        #endregion

        #region resource 

        public delegate void MonoResourceCallback(ResourceType resourceType, UIntPtr value, int is_soft);

        [DllImport(lib)]
        public static extern int mono_runtime_resource_limit(ResourceType resource_type, UIntPtr soft_limit, UIntPtr hard_limit);

        [DllImport(lib)]
        public static extern void mono_runtime_resource_set_callback(MonoResourceCallback callback);

        [DllImport(lib)]
        public static extern void mono_runtime_resource_check_limit(ResourceType resource_type, UIntPtr value);

        #endregion
    }
}
