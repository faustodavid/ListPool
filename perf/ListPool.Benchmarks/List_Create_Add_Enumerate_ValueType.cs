﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class List_Create_Add_Enumerate_ValueType
    {
        [Params(48, 1_024, 10_240)]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public int List()
        {
            int count = 0;
            List<int> list = new List<int>(N);
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
            using ListPool<int> list = new ListPool<int>(N);
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
            using ValueListPool<int> list = N <= 1024
                ? new ValueListPool<int>(stackalloc int[N], ValueListPool<int>.SourceType.UseAsInitialBuffer)
                : new ValueListPool<int>(N);

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
    }
}
