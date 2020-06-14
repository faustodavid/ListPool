using System;
using System.Text.Json.Serialization;
using ListPool.Serializers.System.Text.Json.Converters;
using ListPool.Serializers.Utf8Json.Formatters;
using Utf8Json;

namespace ListPool.Benchmarks.Serializers.Formatters.FakeClasses
{
    public struct FakeClassUsingListPool : IDisposable
    {
        [JsonConverter(typeof(StringAsListPoolOfCharsConverter))]
        [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
        public ListPool<char> Text { get; set; }

        public void Dispose() => Text?.Dispose();
    }
}
