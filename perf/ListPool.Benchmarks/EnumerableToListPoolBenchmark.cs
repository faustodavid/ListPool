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
        [Params(10, 50 ,100, 1000)]
        public int N { get; set; }

        [Params(1)]
        public double CapacityFilled { get; set; }

        private IEnumerable<int> _items;

        [IterationSetup]
        public void GlobalSetup()
        {
            var items = new int[N];

            for (int i = 0; i < N * CapacityFilled; i++)
            {
                items[i] = 1;
            }

            _items = items.Select(i => i);
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _ = _items.ToList();
        }

        [Benchmark]
        public void ListPool()
        {
            using var listPool = _items.ToListPool();
        }
    }
}