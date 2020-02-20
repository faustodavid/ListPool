using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ListPool.Resolvers.Utf8Json;
using Utf8Json;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Utf8JsonSerializeListOfIntBenchmarks
    {
        private readonly ListPoolResolver _resolver = new ListPoolResolver();
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(1_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var items = Enumerable.Range(0, N).ToArray();
            _listPool = items.ToListPool();
            _list = items.ToList();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public int List_SystemTextJson()
        {
            int count = 0;
            List<int> list = _list;

            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;

            return count;
        }

        [Benchmark]
        public int List_utf8json()
        {
            int count = 0;
            List<int> list = _list;

            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;

            return count;
        }


        [Benchmark]
        public int ListPool_utf8json_with_resolver()
        {
            int count = 0;
            ListPoolResolver resolver = _resolver;
            ListPool<int> list = _listPool;

            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;
            count += JsonSerializer.Serialize(list, resolver).Length;

            return count;
        }

        [Benchmark]
        public int ListPool_utf8json()
        {
            int count = 0;
            ListPoolResolver resolver = _resolver;
            ListPool<int> list = _listPool;

            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;
            count += JsonSerializer.Serialize(list).Length;

            return count;
        }

        [Benchmark]
        public int ListPool_SystemTextJson()
        {
            int count = 0;
            ListPoolResolver resolver = _resolver;
            ListPool<int> list = _listPool;

            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;
            count += System.Text.Json.JsonSerializer.Serialize(list).Length;

            return count;
        }

        [Benchmark]
        public int ListPool_ListPoolUtf8Json()
        {
            int count = 0;
            ListPoolResolver resolver = _resolver;
            ListPool<int> list = _listPool;

            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;
            count += Utf8Json.JsonSerializer.Serialize(list).Length;

            return count;
        }
    }
}
