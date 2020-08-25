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
        private int[] _dst;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _dst = new int[N];

            for (int i = 0; i < N; i++)
            {
                _list.Add(i);
                _listPool.Add(i);
            }
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _list.CopyTo(_dst, 0);
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.CopyTo(_dst, 0);
        }
    }
}
