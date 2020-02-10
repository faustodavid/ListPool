using BenchmarkDotNet.Running;

namespace ListPool.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            }
        }
    }
}
