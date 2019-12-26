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

        private List<int> _list;
        private ListPool<int> _listPool;

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);

            for (int i = 1; i <= N; i++)
            {
                _list.Add(i);
                _listPool.Add(i);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _list.RemoveAt(N / 2);
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.RemoveAt(N / 2);
        }
    }
}
