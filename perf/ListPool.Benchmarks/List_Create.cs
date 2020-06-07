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
    public class List_Create
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
            using ListPool<int> list = new ListPool<int>(N);
        }

        [Benchmark]
        public void ValueListPool()
        {
            using ValueListPool<int> list = new ValueListPool<int>(N);
        }
    }
}
