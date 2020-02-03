using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Collections.Pooled;

namespace ListPool.Benchmarks
{
    [RPlotExporter, RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Utf8JsonDeserializeListOfIntBenchmarks
    {
        private byte[] _serializedList;

        [Params(1_000_000)]
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

        [Benchmark]
        public int PooledList()
        {
            using PooledList<int> list = Utf8Json.JsonSerializer.Deserialize<PooledList<int>>(_serializedList);
            return list.Count;
        }
    }
}
