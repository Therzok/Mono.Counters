using System;
using System.Collections.Generic;
using Xunit;

using Mono.Counters;
using System.Collections;

namespace Tests
{
    public class MonoCountersTests
    {
        readonly MonoCounters counters = new MonoCounters();

        [Fact]
        public void All()
        {
            var visitor = new GatheringVisitor();
            counters.Dump(visitor);

            Assert.NotEmpty(visitor);
            Assert.Contains(visitor, x => x.Name == "Major sweep");
            Assert.Contains(visitor, x => x.Name == "User Time");
            Assert.Contains(visitor, x => x.Name == "Major GC time");
        }

        [Fact]
        public void FilterSection()
        {
            var visitor = new GcSectionFilteringVisitor();
            counters.Dump(visitor);

            Assert.NotEmpty(visitor);
            Assert.Contains(visitor, x => x.Name == "Major sweep");
            Assert.DoesNotContain(visitor, x => x.Name == "User Time");
        }

        [Fact]
        public void FilterUnit()
        {
            var visitor = new CountUnitFilteringVisitor();
            counters.Dump(visitor);

            Assert.NotEmpty(visitor);
            Assert.Contains(visitor, x => x.Name == "Page Faults");
            Assert.DoesNotContain(visitor, x => x.Name == "User Time");
        }

        [Fact]
        public void TimeCountersReportTimeSpan()
        {
            var visitor = new TimeUnitVisitor();
            counters.Dump(visitor);

            Assert.NotEmpty(visitor);
            Assert.All(visitor, x => Assert.IsType<TimeSpan>(x.Value));
        }

        class TimeUnitVisitor : GatheringVisitor
        {
            protected override bool Filter(NativeCounter c) => c.Unit == CounterUnit.Time;
        }


        class CountUnitFilteringVisitor : GatheringVisitor
        {
            protected override bool Filter(NativeCounter c) => c.Unit == CounterUnit.Count;
        }

        class GcSectionFilteringVisitor : GatheringVisitor
        {
            protected override bool Filter(NativeCounter c) => c.Section == CounterSection.Gc;
        }

        class GatheringVisitor : CounterVisitor, IEnumerable<Counter>
        {
            public List<Counter> Values { get; } = new List<Counter>();

            protected override void Process(Counter c) => Values.Add(c);

            public IEnumerator<Counter> GetEnumerator() => Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
