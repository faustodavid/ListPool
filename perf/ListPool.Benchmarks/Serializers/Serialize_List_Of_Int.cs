using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ListPool.Serializers.Utf8Json.Resolvers;
using Utf8Json;

namespace ListPool.Benchmarks.Serializers
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Serialize_List_Of_Int
    {
        private readonly ListPoolResolver _resolver = new ListPoolResolver();
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            int[] items = Enumerable.Range(0, N).ToArray();
            _listPool = items.ToListPool();
            _list = items.ToList();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public int Utf8Json_List()
        {
            int count = 0;
            List<int> list = _list;

            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;
            count += JsonSerializer.Serialize(list, _resolver).Length;

            return count;
        }


        [Benchmark]
        public int Utf8Json_ListPool_with_resolver()
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
        public int Utf8Json_ListPool()
        {
            int count = 0;
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
        public int STJ_List()
        {
            int count = 0;
            List<int> list = _list;

            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;

            return count;
        }

        [Benchmark]
        public int STJ_ListPool()
        {
            int count = 0;
            ListPool<int> list = _listPool;

            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;
            count += System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list).Length;

            return count;
        }
    }
}
