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
    public class ListPoolRemoveAtBenchmarks
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
            list.RemoveAt(0);
            list.RemoveAt(N / 2);
            list.RemoveAt(N - 3);
        }

        [Benchmark]
        public void ListPool()
        {
            listPool.RemoveAt(0);
            listPool.RemoveAt(N / 2);
            listPool.RemoveAt(N - 3);
        }
    }
}
