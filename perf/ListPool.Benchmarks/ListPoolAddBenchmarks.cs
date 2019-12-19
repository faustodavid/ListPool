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
        private List<int> list;
        private ListPool<int> listPool;

        [Params(10, 100, 1000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            list = new List<int>(N);
            listPool = new ListPool<int>(N);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            for (var i = 0; i < N - 1; i++)
            {
                list.Add(i);
            }
        }

        [Benchmark]
        public void ListPool()
        {
            for (var i = 0; i < N - 1; i++)
            {
                listPool.Add(i);
            }
        }
    }
}