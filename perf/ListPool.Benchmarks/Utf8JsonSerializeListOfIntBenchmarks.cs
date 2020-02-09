using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
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
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(100, 1_000, 10_000)]
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
        public int List()
        {
            byte[] serializedItems = JsonSerializer.Serialize(_list);
            return serializedItems.Length;
        }

        [Benchmark]
        public int ListPool()
        {
            byte[] serializedItems = JsonSerializer.Serialize(_listPool);
            return serializedItems.Length;
        }


        [Benchmark]
        public int ListPool_Spreads()
        {
            byte[] serializedItems = Spreads.Serialization.Utf8Json.JsonSerializer.Serialize(_listPool);
            return serializedItems.Length;
        }
    }
}
