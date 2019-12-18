using System;
using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ListPoolCreateAndInsertBenchmarks>();
            BenchmarkRunner.Run<ListPoolCreateBenchmarks>();
            BenchmarkRunner.Run<ListPoolEnumerateBenchmarks>();
            BenchmarkRunner.Run<ToListPoolBenchmark>();

            Console.ReadLine();
        }
    }
}
