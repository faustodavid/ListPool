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
        private ValueListPool<int> _valueListPool;
        private int[] _listCopy;

        [Params(100, 1000, 10000)]
        public int N { get; set; }

        [GlobalSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _valueListPool = new ValueListPool<int>(N);
            _listCopy = new int[N];

            for (int i = 0; i < N; i++)
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
            _listPool.CopyTo(_listCopy, 0);
        }

        [Benchmark]
        public void ValueListPool()
        {
            _valueListPool.CopyTo(_listCopy, 0);
        }
    }
}
