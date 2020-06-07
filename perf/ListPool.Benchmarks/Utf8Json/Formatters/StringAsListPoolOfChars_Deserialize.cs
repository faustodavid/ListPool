using System.Text;
using BenchmarkDotNet.Attributes;
using Utf8Json;

namespace ListPool.Benchmarks.Formatters.Utf8Json
{
    [MemoryDiagnoser]
    public class StringAsListPoolOfChars_Deserialize
    {
        private byte[] _json;

        [Params(100, 1_000, 10_000)] public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            StringBuilder sb = new StringBuilder(N + 10);

            sb.Append("{\"Text\":\"");
            for (int i = 0; i < N; i++)
            {
                sb.Append("a");
            }
            sb.Append("\"}");

            _json = Encoding.UTF8.GetBytes(sb.ToString());
        }

        [Benchmark(Baseline = true)]
        public int String() => JsonSerializer.Deserialize<FakeClass>(_json).Text.Length;

        [Benchmark]
        public int ListPool()
        {
            using var dummyClass = JsonSerializer.Deserialize<FakeClassUsingListPool>(_json);
            return dummyClass.Text.Count;
        }
    }
}
