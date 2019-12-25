using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class EnumerableToListPoolBenchmark
    {
        private IEnumerable<int> _items;

        [Params(10, 50, 100, 1000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            var items = new int[N];

            for (int i = 0; i < N - 1; i++)
            {
                items[i] = 1;
            }

            _items = items.Select(i => i);
        }

        [Benchmark]
        public void ListPool()
        {
            using var _ = new ListPool<int>(_items);
        }

        [Benchmark(Baseline = true)]
        public void Linq()
        {
            var _ = _items.ToList();
        }
    }
}
