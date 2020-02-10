using System.Linq;
using AutoFixture;
using Utf8Json;
using Xunit;

namespace ListPool.Netstandard2_0.UnitTests.ListPool.Serializer
{
    public class ListPoolUtf8JsonTests : ListPoolSerializerTestsBase
    {
        public override void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            string serializedItems = JsonSerializer.ToJsonString(expectedItems);

            using ListPool<int> actualItems = JsonSerializer.Deserialize<ListPool<int>>(serializedItems);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        public override void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            string serializedItems = JsonSerializer.ToJsonString(expectedItems);

            using ListPool<CustomObject> actualItems =
                JsonSerializer.Deserialize<ListPool<CustomObject>>(serializedItems);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems,
                expectedItem => actualItems.Any(actualItem => actualItem.Property == expectedItem.Property));
        }

        public override void Serialize_and_deserialize_objects_containing_ListPool()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            var expectedObject = new CustomObjectWithListPool
            {
                Property = s_fixture.Create<string>(), List = expectedItems
            };
            string serializedItems = JsonSerializer.ToJsonString(expectedObject);

            using CustomObjectWithListPool actualObject =
                JsonSerializer.Deserialize<CustomObjectWithListPool>(serializedItems);

            Assert.Equal(expectedObject.Property, actualObject.Property);
            Assert.Equal(expectedItems.Count, actualObject.List.Count);
            Assert.All(expectedItems,
                expectedItem => actualObject.List.Any(actualItem => actualItem == expectedItem));
        }
    }
}
