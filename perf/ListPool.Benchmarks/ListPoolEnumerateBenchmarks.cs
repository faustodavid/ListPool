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
    public class ListPoolEnumerateBenchmarks
    {
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(1000)]
        public int N { get; set; }

        [Params(0.10, 0.50, 0.80, 1)]
        public double CapacityFilled { get; set; }

        [IterationSetup]
        public void GlobalSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);

            for (int i = 0; i < N * CapacityFilled; i++)
            {
                _list.Add(1);
                _listPool.Add(1);
            }
        }

        [IterationCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            foreach (int _ in _list)
            {
            }
        }

        [Benchmark]
        public void ListPool()
        {
            foreach (int _ in _listPool)
            {
            }
        }
    }
}
