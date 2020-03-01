using System.Linq;
using AutoFixture;
using Utf8Json;
using Xunit;

namespace ListPool.Resolvers.Utf8Json.Tests
{
    public class ListPoolResolverTests
    {
        protected static readonly Fixture s_fixture = new Fixture();
        private readonly ListPoolResolver _sut = new ListPoolResolver();

        [Fact]
        public void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            byte[] serializedItems = JsonSerializer.Serialize(expectedItems, _sut);

            using ListPool<int> actualItems = JsonSerializer.Deserialize<ListPool<int>>(serializedItems, _sut);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        [Fact]
        public void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            byte[] serializedItems = JsonSerializer.Serialize(expectedItems, _sut);

            using ListPool<CustomObject> actualItems =
                JsonSerializer.Deserialize<ListPool<CustomObject>>(serializedItems, _sut);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems,
                expectedItem => actualItems.Any(actualItem => actualItem.Property == expectedItem.Property));
        }

        [Fact]
        public void Serialize_and_deserialize_objects_containing_ListPool()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            CustomObjectWithListPool expectedObject = new CustomObjectWithListPool
            {
                Property = s_fixture.Create<string>(), List = expectedItems
            };
            byte[] serializedItems = JsonSerializer.Serialize(expectedObject, _sut);

            using CustomObjectWithListPool actualObject =
                JsonSerializer.Deserialize<CustomObjectWithListPool>(serializedItems, _sut);

            Assert.Equal(expectedObject.Property, actualObject.Property);
            Assert.Equal(expectedItems.Count, actualObject.List.Count);
            Assert.All(expectedItems,
                expectedItem => actualObject.List.Any(actualItem => actualItem == expectedItem));
        }
    }
}
