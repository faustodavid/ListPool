using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class ListPoolCreateBenchmarks
    {
        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _ = new List<int>(N);
        }

        [Benchmark]
        public void ListPool()
        {
            using var list = new ListPool<int>(N);
        }

        [Benchmark]
        public void ValueListPool()
        {
            using var list = new ValueListPool<int>(N);
        }
    }
}
