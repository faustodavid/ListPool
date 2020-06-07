using System;
using ListPool.Formatters.Utf8Json;
using Utf8Json;

namespace ListPool.Benchmarks.Formatters.Utf8Json
{
    public class DummyClassUsingListPool : IDisposable
    {
        [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
        public ListPool<char> Text { get; set; }

        public void Dispose() => Text?.Dispose();
    }
}
