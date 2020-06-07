using System.Text;
using BenchmarkDotNet.Attributes;
using Utf8Json;

namespace ListPool.Benchmarks.Formatters.Utf8Json
{
    [MemoryDiagnoser]
    public class StringAsListPoolOfChars_Serialize
    {
        [Params(100, 1_000, 10_000)] public int N { get; set; }

        private readonly FakeClass _fakeClass = new FakeClass();
        private readonly FakeClassUsingListPool _fakeClassUsingListPool = new FakeClassUsingListPool();

        [GlobalSetup]
        public void GlobalSetup()
        {
            StringBuilder sb = new StringBuilder(N);
            for (int i = 0; i < N; i++)
            {
                sb.Append("a");
            }

            _fakeClass.Text = sb.ToString();
            _fakeClassUsingListPool.Text?.Dispose();
            _fakeClassUsingListPool.Text = new ListPool<char>(_fakeClass.Text.ToCharArray());
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _fakeClassUsingListPool.Text?.Dispose();
            _fakeClassUsingListPool.Text = null;
        }

        [Benchmark(Baseline = true)]
        public byte[] String() => JsonSerializer.Serialize(_fakeClass);

        [Benchmark]
        public byte[] ListPool() => JsonSerializer.Serialize(_fakeClassUsingListPool);
    }
}
