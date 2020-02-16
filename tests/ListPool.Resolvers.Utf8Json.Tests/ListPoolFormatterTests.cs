using System.Linq;
using AutoFixture;
using ListPool.Resolvers.Utf8Json;
using Xunit;

namespace ListPool.Formatters.Utf8Json.Tests
{
    public class ListPoolFormatterTests
    {
        protected static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            ListPoolFormatter<CustomObject> sut = new ListPoolFormatter<CustomObject>();
            byte[] serializedItems = sut.Serialize(expectedItems);

            using ListPool<CustomObject> actualItems = sut.Deserialize(serializedItems);

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
            byte[] serializedItems = sut.Serialize(expectedItems);

            using ListPool<int> actualItems = sut.Deserialize(serializedItems);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }
    }
}
