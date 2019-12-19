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
    public class ListPoolIndexOfBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        private List<int> list;
        private ListPool<int> listPool;

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
            list.IndexOf(1);
            list.IndexOf(N / 2);
            list.IndexOf(N);
            list.IndexOf(-1);
        }

        [Benchmark]
        public void ListPool()
        {
            listPool.IndexOf(1);
            listPool.IndexOf(N / 2);
            listPool.IndexOf(N);
            listPool.IndexOf(-1);
        }
    }
}
