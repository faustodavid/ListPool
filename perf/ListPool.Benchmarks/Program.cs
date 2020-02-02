using System;
using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            Console.ReadLine();
        }
    }
}
