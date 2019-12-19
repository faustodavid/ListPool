using System;
using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<ListPoolAddBenchmarks>();
            //BenchmarkRunner.Run<ListPoolClearBenchmarks>();
            //BenchmarkRunner.Run<ListPoolContainsBenchmark>();
            //BenchmarkRunner.Run<ListPoolCopyToBenchmarks>();
            //BenchmarkRunner.Run<ListPoolCreateAndAddBenchmarks>();
            //BenchmarkRunner.Run<ListPoolCreateBenchmarks>();
            //BenchmarkRunner.Run<ListPoolEnumerateBenchmarks>();
            //BenchmarkRunner.Run<ListPoolIndexOfBenchmarks>();
            //BenchmarkRunner.Run<ListPoolInsertBenchmarks>();
            //BenchmarkRunner.Run<ListPoolRemoveAtBenchmarks>();
            //BenchmarkRunner.Run<ListPoolRemoveBenchmarks>();
            BenchmarkRunner.Run<ToListPoolBenchmark>();

            Console.ReadLine();
        }
    }
}
