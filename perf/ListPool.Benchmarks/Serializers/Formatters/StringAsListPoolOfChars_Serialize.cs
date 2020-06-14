using System.Text;
using BenchmarkDotNet.Attributes;
using ListPool.Benchmarks.Serializers.Formatters.FakeClasses;
using Utf8Json;

namespace ListPool.Benchmarks.Serializers.Formatters
{
    [MemoryDiagnoser]
    public class StringAsListPoolOfChars_Serialize
    {
        [Params(100, 1_000, 10_000)] public int N { get; set; }

        private FakeClass _fakeClass = new FakeClass();
        private FakeClassUsingListPool _fakeClassUsingListPool = new FakeClassUsingListPool();

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
        public byte[] Utf8Json_String() => JsonSerializer.Serialize(_fakeClass);

        [Benchmark]
        public byte[] STJ_String() => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(_fakeClass);

        [Benchmark]
        public byte[] Utf8Json_ListPool() => JsonSerializer.Serialize(_fakeClassUsingListPool);

        [Benchmark]
        public byte[] STJ_ListPool() => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(_fakeClassUsingListPool);
    }
}
