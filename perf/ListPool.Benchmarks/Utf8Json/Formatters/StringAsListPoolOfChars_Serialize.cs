using System.Text;
using BenchmarkDotNet.Attributes;
using Utf8Json;

namespace ListPool.Benchmarks.Formatters.Utf8Json
{
    [MemoryDiagnoser]
    public class StringAsListPoolOfChars_Serialize
    {
        [Params(100, 1_000, 10_000)] public int N { get; set; }

        private readonly DummyClass _dummyClass = new DummyClass();
        private readonly DummyClassUsingListPool _dummyClassUsingListPool = new DummyClassUsingListPool();

        [GlobalSetup]
        public void GlobalSetup()
        {
            StringBuilder sb = new StringBuilder(N);
            for (int i = 0; i < N; i++)
            {
                sb.Append("a");
            }

            _dummyClass.Text = sb.ToString();
            _dummyClassUsingListPool.Text?.Dispose();
            _dummyClassUsingListPool.Text = new ListPool<char>(_dummyClass.Text.ToCharArray());
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _dummyClassUsingListPool.Text?.Dispose();
            _dummyClassUsingListPool.Text = null;
        }

        [Benchmark(Baseline = true)]
        public byte[] String() => JsonSerializer.Serialize(_dummyClass);

        [Benchmark]
        public byte[] ListPool() => JsonSerializer.Serialize(_dummyClassUsingListPool);
    }
}
