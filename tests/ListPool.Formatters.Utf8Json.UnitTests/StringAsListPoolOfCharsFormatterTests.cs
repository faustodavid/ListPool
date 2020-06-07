using System;
using System.Text;
using AutoFixture;
using Utf8Json;
using Xunit;

namespace ListPool.Formatters.Utf8Json.UnitTests
{
    public class StringAsListPoolOfCharsFormatterTests
    {
        private static readonly Fixture _fixture = new Fixture();

        [Fact]
        public void Serialize_ListPool_of_chars_as_string()
        {
            string fakeText = _fixture.Create<string>();
            string expectedJson = string.Concat("{\"Text\":\"", fakeText, "\"}");
            using var fakeClass = new FakeClass { Text = new ListPool<char>() };
            fakeClass.Text.AddRange(fakeText.AsSpan());

            string actualJson = JsonSerializer.ToJsonString(fakeClass);

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void Deserialize_string_as_ListPool_of_chars()
        {
            string expectedText = _fixture.Create<string>();
            string json = string.Concat("{\"Text\":\"", expectedText, "\"}");

            using FakeClass fakeClass = JsonSerializer.Deserialize<FakeClass>(Encoding.UTF8.GetBytes(json));
            ListPool<char> actualText = fakeClass.Text;

            Assert.Equal(expectedText.Length, actualText.Count);
            for (int i = 0; i < expectedText.Length; i++)
            {
                Assert.Equal(expectedText[i], actualText[i]);
            }
        }

        public class FakeClass : IDisposable
        {
            [JsonFormatter(typeof(StringAsListPoolOfCharsFormatter))]
            public ListPool<char> Text { get; set; }

            public void Dispose() => Text?.Dispose();
        }
    }
}
