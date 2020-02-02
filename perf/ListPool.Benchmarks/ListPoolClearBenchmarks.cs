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
    public class ListPoolClearBenchmarks
    {
        private List<int> _list;
        private ListPool<int> _listPool;
        private ValueListPool<int> _valueListPool;

        [Params(1000)]
        public int N { get; set; }

        [Params(0.10, 0.50, 0.80, 1)]
        public double CapacityFilled { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _valueListPool = new ValueListPool<int>(N);

            for (int i = 0; i < N * CapacityFilled; i++)
            {
                _list.Add(1);
                _listPool.Add(1);
                _valueListPool.Add(1);
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
            _list.Clear();
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.Clear();
        }

        [Benchmark]
        public void ValueListPool()
        {
            _valueListPool.Clear();
        }
    }
}
