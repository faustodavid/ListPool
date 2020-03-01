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
        private IEnumerable<int> _items => Enumerable.Range(0, N);

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [Benchmark]
        public void ListPool()
        {
            using ListPool<int> _ = _items.ToListPool();
        }

        [Benchmark(Baseline = true)]
        public void Linq()
        {
            _ = _items.ToList();
        }
    }
}
