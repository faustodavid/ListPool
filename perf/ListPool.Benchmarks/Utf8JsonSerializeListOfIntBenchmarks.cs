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

        [Params(100, 1_000, 10_000)]
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
            _pooledList.Dispose();
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
        public int PooledList()
        {
            byte[] serializedItems = JsonSerializer.Serialize(_pooledList);
            return serializedItems.Length;
        }
    }
}
