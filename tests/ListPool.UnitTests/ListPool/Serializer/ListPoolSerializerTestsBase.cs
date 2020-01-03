using AutoFixture;
using Xunit;

namespace ListPool.UnitTests.ListPool.Serializer
{
    public abstract class ListPoolSerializerTestsBase
    {
        protected static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public abstract void Serialize_and_deserialize_ListPool_with_value_types();

        [Fact]
        public abstract void Serialize_and_deserialize_ListPool_with_objects();

        [Fact]
        public abstract void Serialize_and_deserialize_objects_containing_ListPool();
    }
}
