using System.Linq;
using AutoFixture;
using Newtonsoft.Json;
using Xunit;

namespace ListPool.UnitTests.ListPool.Serializer
{
    public class ListPoolNewtonsoftTests : ListPoolSerializerTestsBase
    {
        public override void Serialize_and_deserialize_ListPool_with_value_types()
        {
            using ListPool<int> expectedItems = new ListPool<int>
            {
                s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()
            };
            string serializedItems = JsonConvert.SerializeObject(expectedItems);

            using ListPool<int> actualItems = JsonConvert.DeserializeObject<ListPool<int>>(serializedItems);

            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.All(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        public override void Serialize_and_deserialize_ListPool_with_objects()
        {
            using ListPool<CustomObject> expectedItems = new ListPool<CustomObject>
            {
                s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>(), s_fixture.Create<CustomObject>()
            };
            string serializedItems = JsonConvert.SerializeObject(expectedItems);

            using ListPool<CustomObject> actualItems =
                JsonConvert.DeserializeObject<ListPool<CustomObject>>(serializedItems);

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
            string serializedItems = JsonConvert.SerializeObject(expectedObject);

            using CustomObjectWithListPool actualObject =
                JsonConvert.DeserializeObject<CustomObjectWithListPool>(serializedItems);

            Assert.Equal(expectedObject.Property, actualObject.Property);
            Assert.Equal(expectedItems.Count, actualObject.List.Count);
            Assert.All(expectedItems,
                expectedItem => actualObject.List.Any(actualItem => actualItem == expectedItem));
        }
    }
}
