using System;
namespace Mono.Counters
{
    public enum ResourceType
    {
        JitCode, /* bytes */
        Metadata, /* bytes */
        GcHeap,  /* bytes */
    }
}
