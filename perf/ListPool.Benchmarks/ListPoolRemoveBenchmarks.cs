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
    public class ListPoolRemoveBenchmarks
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
            _list.Remove(1);
            _list.Remove(N / 2);
            _list.Remove(N);
            _list.Remove(-1);
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.Remove(1);
            _listPool.Remove(N / 2);
            _listPool.Remove(N);
            _listPool.Remove(-1);
        }
    }
}
