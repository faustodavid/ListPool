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
        private List<int> _list;
        private ListPool<int> _listPool;
        private int[] _listCopy;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _listCopy = new int[N];

            for (int i = 0; i < N; i++)
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
            _listPool.CopyTo(_listCopy, 0);
        }
    }
}
