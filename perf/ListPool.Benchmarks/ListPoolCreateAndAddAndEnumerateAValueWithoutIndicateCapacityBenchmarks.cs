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
    public class ListPoolCreateAndAddAndEnumerateAValueWithoutIndicateCapacityBenchmarks
    {
        [Params(48, 1_024, 10_240)]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public int List()
        {
            int count = 0;
            List<int> list = new List<int>();
            for (int i = 0; i < N; i += 8)
            {
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
            }

            foreach (int item in list)
            {
                count += item;
            }

            return count;
        }

        [Benchmark]
        public int ListPool()
        {
            int count = 0;
            using ListPool<int> list = new ListPool<int>();
            for (int i = 0; i < N; i += 8)
            {
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
            }

            foreach (int item in list)
            {
                count += item;
            }

            return count;
        }

        [Benchmark]
        public int ValueListPool()
        {
            int count = 0;
            using ValueListPool<int> list = new ValueListPool<int>(0);

            for (int i = 0; i < N;  i += 8)
            {
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
            }

            foreach (int item in list)
            {
                count += item;
            }

            return count;
        }
    }
}
