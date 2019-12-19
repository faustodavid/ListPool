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
    public class ListPoolCopyToBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        private List<int> list;
        private ListPool<int> listPool;
        private int[] listCopy;
        private int[] listPoolCopy;

        [IterationSetup]
        public void IterationSetup()
        {
            list = new List<int>(N);
            listPool = new ListPool<int>(N);
            listCopy = new int[N];
            listPoolCopy = new int[N];

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
            listPool.CopyTo(listCopy, 0);
        }

        [Benchmark]
        public void ListPool()
        {
            listPool.CopyTo(listPoolCopy, 0);
        }
    }
}
