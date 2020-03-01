using System.Linq;
using System.Text.Json;
using AutoFixture;
using Xunit;

namespace ListPool.Netstandard2_0.UnitTests.ListPool.Serializer
{
    public class ListPoolSystemTextJsonSerializerTests : ListPoolSerializerTestsBase
    {
        public override void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            string serializedItems = JsonSerializer.Serialize(expectedItems);

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
            string serializedItems = JsonSerializer.Serialize(expectedItems);

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
            CustomObjectWithListPool expectedObject = new CustomObjectWithListPool
            {
                Property = s_fixture.Create<string>(), List = expectedItems
            };
            string serializedItems = JsonSerializer.Serialize(expectedObject);

            using CustomObjectWithListPool actualObject =
                JsonSerializer.Deserialize<CustomObjectWithListPool>(serializedItems);

            Assert.Equal(expectedObject.Property, actualObject.Property);
            Assert.Equal(expectedItems.Count, actualObject.List.Count);
            Assert.All(expectedItems,
                expectedItem => actualObject.List.Any(actualItem => actualItem == expectedItem));
        }
    }
}
