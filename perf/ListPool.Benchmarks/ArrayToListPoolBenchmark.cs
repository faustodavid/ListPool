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
        [Params(10, 50 ,100, 1000)]
        public int N { get; set; }

        private int[] array;

        [GlobalSetup]
        public void GlobalSetup()
        {
            array = new int[N];

            for (int i = 0; i < N - 1; i++)
            {
                array[i] = 1;
            }
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _ = array.ToList();
        }

        [Benchmark]
        public void ListPool()
        {
            using var _ = array.ToListPool();
        }
    }
}