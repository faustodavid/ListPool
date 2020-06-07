using System;
using ListPool.Formatters.Utf8Json;
using Utf8Json;

namespace ListPool.Benchmarks.Formatters.Utf8Json
{
    public struct FakeClassUsingListPool : IDisposable
    {
        [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
        public ListPool<char> Text { get; set; }

        public void Dispose() => Text?.Dispose();
    }
}
