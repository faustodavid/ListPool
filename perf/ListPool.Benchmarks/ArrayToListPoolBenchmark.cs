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
    public class ArrayToListPoolBenchmark
    {
        private int[] _array;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _array = new int[N];

            for (int i = 0; i < N - 1; i++)
            {
                _array[i] = 1;
            }
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _ = _array.ToList();
        }

        [Benchmark]
        public void ListPool()
        {
            using ListPool<int> _ = _array.ToListPool();
        }

        [Benchmark]
        public void ValueListPool()
        {
            using ValueListPool<int> _ = _array.ToValueListPool();
        }
    }
}
