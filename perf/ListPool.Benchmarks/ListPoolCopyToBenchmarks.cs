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
        private ValueListPool<int> _valueListPool;
        private int[] _listPoolValueCopy;

        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _valueListPool = new ValueListPool<int>(N);
            _listCopy = new int[N];
            _listPoolCopy = new int[N];
            _listPoolValueCopy = new int[N];

            for (int i = 1; i <= N; i++)
            {
                _list.Add(i);
                _listPool.Add(i);
                _valueListPool.Add(i);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _listPool.Dispose();
            _valueListPool.Dispose();
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
            _valueListPool.CopyTo(_listPoolCopy, 0);
        }
    }
}
