using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Utf8JsonDeserializeListOfIntBenchmarks
    {
        private byte[] _serializedList;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _serializedList = Utf8Json.JsonSerializer.Serialize(Enumerable.Range(0, N));
        }

        [Benchmark(Baseline = true)]
        public int List()
        {
            List<int> list = Utf8Json.JsonSerializer.Deserialize<List<int>>(_serializedList);
            return list.Count;
        }

        [Benchmark]
        public int ListPool()
        {
            using ListPool<int> list = Utf8Json.JsonSerializer.Deserialize<ListPool<int>>(_serializedList);
            return list.Count;
        }

        //[Benchmark]
        //public int ListPool_Spreads()
        //{
        //    using ListPool<int> list =
        //        Spreads.Serialization.Utf8Json.JsonSerializer.Deserialize<ListPool<int>>(_serializedList);
        //    return list.Count;
        //}

        //[Benchmark]
        //public int List_Spreads()
        //{
        //    List<int> list = Spreads.Serialization.Utf8Json.JsonSerializer.Deserialize<List<int>>(_serializedList);
        //    return list.Count;
        //}
    }
}
