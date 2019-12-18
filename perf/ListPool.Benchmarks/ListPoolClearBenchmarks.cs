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
    public class ListPoolClearBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public void List()
        {
            var list = new List<int>(N);
            list.Clear();
        }

        [Benchmark]
        public void ListPool()
        {
            using var list = new ListPool<int>(N);
            list.Clear();
        }
    }
}
