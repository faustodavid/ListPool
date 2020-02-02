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
    public class ListPoolAddBenchmarks
    {
        private List<int> _list;
        private ListPool<int> _listPool;
        private ValueListPool<int> _valueListPool;

        [Params(10, 100, 1000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _valueListPool = new ValueListPool<int>(N);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
            _valueListPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            for (int i = 0; i < N - 1; i++)
            {
                _list.Add(i);
            }
        }

        [Benchmark]
        public void ListPool()
        {
            for (int i = 0; i < N - 1; i++)
            {
                _listPool.Add(i);
            }
        }

        [Benchmark]
        public void ValueListPool()
        {
            for (int i = 0; i < N - 1; i++)
            {
                _valueListPool.Add(i);
            }
        }
    }
}
