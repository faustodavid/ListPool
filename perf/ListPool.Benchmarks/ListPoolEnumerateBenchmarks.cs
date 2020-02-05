using System;
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

        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);

            for (int i = 0; i < N; i++)
            {
                _list.Add(1);
                _listPool.Add(1);
            }
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public int List()
        {
            int count = 0;
            foreach (int value in _list)
            {
                count += value;
            }

            return count;
        }

        [Benchmark]
        public int ListPool()
        {
            int count = 0;
            foreach (int value in _listPool)
            {
                count += value;
            }

            return count;
        }

        [Benchmark]
        public int ListPoolAsSpan()
        {
            int count = 0;
            Span<int> span = _listPool.AsSpan();
            foreach (int value in span)
            {
                count += value;
            }

            return count;
        }
    }
}
