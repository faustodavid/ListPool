using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Collections.Pooled;
using Utf8Json;

namespace ListPool.Benchmarks
{
    [RPlotExporter]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Utf8JsonSerializeListOfIntBenchmarks
    {
        private List<int> _list;
        private ListPool<int> _listPool;
        private PooledList<int> _pooledList;

        [Params(10000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var items = Enumerable.Range(0, N).ToArray();
            _listPool = items.ToListPool();
            _list = items.ToList();
            _pooledList = items.ToPooledList();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public int List()
        {
            string serializedItems = JsonSerializer.ToJsonString(_list);
            return serializedItems.Length;
        }

        [Benchmark]
        public int ListPool()
        {
            string serializedItems = JsonSerializer.ToJsonString(_listPool);
            return serializedItems.Length;
        }

        [Benchmark]
        public int PooledList()
        {
            string serializedItems = JsonSerializer.ToJsonString(_pooledList);
            return serializedItems.Length;
        }
    }
}
