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
    public class EnumerableToListPoolAndEnumerateBenchmark
    {
        private IEnumerable<int> _items;

        [Params(10, 50, 100, 1000)]
        public int N { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            int[] items = new int[N];

            for (int i = 0; i < N - 1; i++)
            {
                items[i] = 1;
            }

            _items = items.Select(i => i);
        }

        [Benchmark]
        public void ListPool()
        {
            using ListPool<int> list = _items.ToListPool();
            foreach (int _ in list)
            {
            }
        }

        [Benchmark]
        public void ListPoolValue()
        {
            using ValueListPool<int> valueList = _items.ToValueListPool();
            foreach (int _ in valueList)
            {
            }
        }

        [Benchmark(Baseline = true)]
        public void Linq()
        {
            List<int> list = _items.ToList();
            foreach (int _ in list)
            {
            }
        }
    }
}
