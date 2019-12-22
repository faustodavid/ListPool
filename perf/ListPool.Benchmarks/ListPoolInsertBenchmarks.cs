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
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(10, 100, 1000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            for (int i = 0; i < N; i++)
            {
                _list.Add(i);
            }
        }

        [Benchmark]
        public void ListPool()
        {
            for (int i = 0; i < N; i++)
            {
                _listPool.Add(i);
            }
        }
    }
}
