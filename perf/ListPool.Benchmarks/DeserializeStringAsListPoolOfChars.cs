using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using ListPool.Formatters.Utf8Json;
using Utf8Json;

namespace ListPool.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializeStringAsListPoolOfChars
    {
        [Params(500, 1000)] public int N { get; set; }

        private DummyClass _dummyClass = new DummyClass();
        private DummyClassUsingListPool _dummyClassUsingListPool = new DummyClassUsingListPool();

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
        public byte[] UsingString() => JsonSerializer.Serialize(_dummyClass);

        [Benchmark]
        public byte[] UsingListPool() => JsonSerializer.Serialize(_dummyClassUsingListPool);
    }

    public class DummyClass
    {
        public string Text { get; set; }
    }

    public class DummyClassUsingListPool : IDisposable
    {
        [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
        public ListPool<char> Text { get; set; }

        public void Dispose() => Text?.Dispose();
    }

    [MemoryDiagnoser]
    public class DeserializeStringAsListPoolOfChars
    {
        private byte[] _json;

        [Params(500, 10000)] public int N { get; set; }

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
        public int UsingString() => JsonSerializer.Deserialize<DummyClass>(_json).Text.Length;

        [Benchmark]
        public int UsingListPool()
        {
            using var dummyClass = JsonSerializer.Deserialize<DummyClassUsingListPool>(_json);
            return dummyClass.Text.Count;
        }
    }
}
