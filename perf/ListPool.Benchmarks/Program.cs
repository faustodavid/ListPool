using System;
using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<ListPoolCreateBenchmarks>();
            //BenchmarkRunner.Run<ListPoolEnumerateBenchmarks>();
            //BenchmarkRunner.Run<ArrayToListPoolBenchmark>();
            BenchmarkRunner.Run<EnumerableToListPoolBenchmark>();
            //BenchmarkRunner.Run<ListPoolClearBenchmarks>();

            Console.ReadLine();
        }
    }
}
