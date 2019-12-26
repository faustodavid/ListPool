using System;
using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<ListPoolCreateBenchmarks>();
            BenchmarkRunner.Run<ListPoolEnumerateBenchmarks>();
            BenchmarkRunner.Run<ArrayToListPoolBenchmark>();
            BenchmarkRunner.Run<EnumerableToListPoolBenchmark>();
            BenchmarkRunner.Run<ListPoolClearBenchmarks>();
            BenchmarkRunner.Run<ListPoolInsertBenchmarks>();

            Console.ReadLine();
        }
    }
}
