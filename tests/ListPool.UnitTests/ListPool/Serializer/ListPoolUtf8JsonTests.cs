using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests.ListPool.Serializer
{
    public class ListPoolUtf8JsonTests : ListPoolSerializerTestsBase
    {
        public override void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            string serializedItems = Utf8Json.JsonSerializer.ToJsonString(expectedItems);

            using ListPool<int> actualItems = Utf8Json.JsonSerializer.Deserialize<ListPool<int>>(serializedItems);

            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        public override void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            string serializedItems = Utf8Json.JsonSerializer.ToJsonString(expectedItems);

            using ListPool<CustomObject> actualItems =
                Utf8Json.JsonSerializer.Deserialize<ListPool<CustomObject>>(serializedItems);

            Assert.All(expectedItems,
                expectedItem => actualItems.Single(actualItem => actualItem.Property == expectedItem.Property));
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
            string serializedItems = Utf8Json.JsonSerializer.ToJsonString(expectedObject);

            using CustomObjectWithListPool actualObject =
                Utf8Json.JsonSerializer.Deserialize<CustomObjectWithListPool>(serializedItems);

            Assert.Equal(expectedObject.Property, actualObject.Property);
            Assert.All(expectedItems,
                expectedItem => actualObject.List.Single(actualItem => actualItem == expectedItem));
        }
    }
}
