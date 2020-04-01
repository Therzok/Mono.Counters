using System;
using System.Diagnostics;

namespace Mono.Counters
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Counter : IEquatable<Counter>
    {
        public string Name { get; }
        public object Value { get; }

        internal Counter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Counter other)
            => Name == other.Name && Value.Equals(other.Value);

        string DebuggerDisplay
            => string.Format("{0}: {1}", Name, Value);
    }
}
