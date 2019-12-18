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
    public class ToListPoolBenchmark
    {
        [Params(1000)]
        public int N { get; set; }

        [Params(0.10, 0.50, 0.80, 1)]
        public double CapacityFilled { get; set; }

        private int[] array;

        [GlobalSetup]
        public void GlobalSetup()
        {
            array = new int[N];

            for (int i = 0; i < N * CapacityFilled; i++)
            {
                array[i] = 1;
            }
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            var list = array.ToList();
        }

        [Benchmark]
        public void ListPool()
        {
            using var listPool = array.ToListPool();
        }
    }
}