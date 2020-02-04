using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Collections.Pooled;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class ValueEnumeratorBenchmarks
    {
        private int[] _items;

        [Params(1_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _items = Enumerable.Range(0, N).ToArray();
        }

        [Benchmark(Baseline = true)]
        public int ValueEnumerator()
        {
            int count = 0;
            using ValueEnumerator<int> enumerator = new ValueEnumerator<int>(_items, _items.Length);
            while (enumerator.MoveNext())
            {
                count += enumerator.Current;
            }

            return count;
        }
    }
}
