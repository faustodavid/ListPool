using System;
using System.Linq;
using AutoFixture;
using ListPool.Serializers.Utf8Json.Formatters;
using Utf8Json;
using Xunit;

namespace ListPool.Formatters.Utf8Json.UnitTests
{
    public class ListPoolFormatterTests
    {
        private static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            ListPoolFormatter<CustomObject> sut = new ListPoolFormatter<CustomObject>();

            JsonWriter writer = new JsonWriter();
            sut.Serialize(ref writer, expectedItems, JsonSerializer.DefaultResolver);
            byte[] serializedItems = writer.GetBuffer().ToArray();
            JsonReader reader = new JsonReader(serializedItems);
            using ListPool<CustomObject> actualItems = sut.Deserialize(ref reader, JsonSerializer.DefaultResolver);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems,
                expectedItem => actualItems.Any(actualItem => actualItem.Property == expectedItem.Property));
        }

        [Fact]
        public void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            ListPoolFormatter<int> sut = new ListPoolFormatter<int>();

            JsonWriter writer = new JsonWriter();
            sut.Serialize(ref writer, expectedItems, JsonSerializer.DefaultResolver);
            byte[] serializedItems = writer.GetBuffer().ToArray();
            JsonReader reader = new JsonReader(serializedItems);
            using ListPool<int> actualItems = sut.Deserialize(ref reader, JsonSerializer.DefaultResolver);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        public class CustomObject
        {
            public string Property { get; set; }
        }

        public sealed class CustomObjectWithListPool : CustomObject, IDisposable
        {
            public ListPool<int> List { get; set; }

            public void Dispose() => List?.Dispose();
        }
    }
}
