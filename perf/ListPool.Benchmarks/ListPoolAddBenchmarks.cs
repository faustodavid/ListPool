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
    public class ListPoolAddBenchmarks
    {
        [Params(100, 1000, 10000)]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public int List()
        {
            List<int> list = new List<int>(N);
            for (int i = 0; i < N; i++)
            {
                list.Add(i);
            }

            return list.Count;
        }

        [Benchmark]
        public int ListPool()
        {
            using ListPool<int> list = new ListPool<int>(N);
            for (int i = 0; i < N; i++)
            {
                list.Add(i);
            }

            return list.Count;
        }

        [Benchmark]
        public int ValueListPool()
        {
            using ValueListPool<int> list = new ValueListPool<int>(N);
            for (int i = 0; i < N; i++)
            {
                list.Add(i);
            }

            return list.Count;
        }
    }
}
