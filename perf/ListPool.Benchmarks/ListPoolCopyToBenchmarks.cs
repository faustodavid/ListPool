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
        private int[] _listCopy;
        private ListPool<int> _listPool;
        private int[] _listPoolCopy;
        private ListPoolValue<int> _listPoolValue;
        private int[] _listPoolValueCopy;

        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _listPoolValue = new ListPoolValue<int>(N);
            _listCopy = new int[N];
            _listPoolCopy = new int[N];
            _listPoolValueCopy = new int[N];

            for (int i = 1; i <= N; i++)
            {
                _list.Add(i);
                _listPool.Add(i);
                _listPoolValue.Add(i);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _listPool.Dispose();
            _listPoolValue.Dispose();
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

        [Benchmark]
        public void ListPoolValue()
        {
            _listPoolValue.CopyTo(_listPoolCopy, 0);
        }
    }
}
