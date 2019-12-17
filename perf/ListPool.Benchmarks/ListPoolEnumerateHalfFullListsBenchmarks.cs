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
        [Params(1000)]
        public int N { get; set; }

        [Params(0.10, 0.50, 0.80, 1)]
        public double CapacityFilled { get; set; }

        private List<int> list;
        private ListPool<int> listPool;

        [GlobalSetup]
        public void GlobalSetup()
        {
            list = new List<int>(N);
            listPool = ListPool<int>.Rent(N);

            for (int i = 0; i < N * CapacityFilled; i++)
            {
                list.Add(1);
                listPool.Add(1);
            }
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            foreach (var item in list)
            {

            }
        }

        [Benchmark]
        public void ListPool()
        {
            foreach (var item in listPool)
            {

            }
        }

        public void Dispose()
        {
            this.listPool.Dispose();
        }
    }
}