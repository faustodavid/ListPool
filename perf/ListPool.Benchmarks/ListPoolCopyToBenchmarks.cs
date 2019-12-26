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

        private List<int> _list;
        private ListPool<int> _listPool;
        private int[] _listCopy;
        private int[] _listPoolCopy;

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _listCopy = new int[N];
            _listPoolCopy = new int[N];

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
            _listPool.CopyTo(_listCopy, 0);
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.CopyTo(_listPoolCopy, 0);
        }
    }
}
