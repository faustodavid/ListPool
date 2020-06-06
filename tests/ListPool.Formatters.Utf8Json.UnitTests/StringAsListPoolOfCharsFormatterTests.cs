using System;
using Utf8Json;
using Xunit;

namespace ListPool.Formatters.Utf8Json.UnitTests
{
    public class StringAsListPoolOfCharsFormatterTests
    {
        public class DummyClassUsingListPool : IDisposable
        {
            [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
            public ListPool<char> Text { get; set; }

            public void Dispose() => Text?.Dispose();
        }

        [Fact]
        public void Test1()
        {
            using var dummyClass = new DummyClassUsingListPool();
            dummyClass.Text = new ListPool<char>();
            dummyClass.Text.AddRange(stackalloc char[4] {'h', 'o', 'l', 'a'});

            string serialized = JsonSerializer.ToJsonString(dummyClass);

            Assert.Equal("{\"Text\":\"hola\"}", serialized);
        }
    }
}
