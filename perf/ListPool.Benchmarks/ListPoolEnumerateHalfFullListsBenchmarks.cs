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
    public class ListPoolEnumerateBenchmarks : IDisposable
    {
        private List<int> list;
        private ListPool<int> listPool;

        [Params(1000)]
        public int N { get; set; }

        [Params(0.10, 0.50, 0.80, 1)]
        public double CapacityFilled { get; set; }

        public void Dispose()
        {
            listPool.Dispose();
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            list = new List<int>(N);
            listPool = new ListPool<int>();

            for (var i = 0; i < N * CapacityFilled; i++)
            {
                list.Add(1);
                listPool.Add(1);
            }
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            foreach (var _ in list)
            {
            }
        }

        [Benchmark]
        public void ListPool()
        {
            foreach (var _ in listPool)
            {
            }
        }
    }
}