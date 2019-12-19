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
    public class ListPoolInsertBenchmarks
    {
        private List<int> list;
        private ListPool<int> listPool;

        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            list = new List<int>(N);
            listPool = new ListPool<int>(N);
            for (var i = 1; i <= N; i++)
            {
                list.Add(i);
                listPool.Add(i);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            list.Insert(1, 11111);
            list.Insert(N / 2, 22222);
            list.Insert(N, 33333);
        }

        [Benchmark]
        public void ListPool()
        {
            listPool.Insert(1, 11111);
            listPool.Insert(N / 2, 22222);
            listPool.Insert(N, 33333);
        }
    }
}