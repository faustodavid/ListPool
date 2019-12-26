using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests
{
    public class ListPoolTests
    {
        static ListPoolTests()
        {
            s_fixture = new Fixture();
        }

        private static readonly Fixture s_fixture;

        [Fact]
        public void Add_items_when_capacity_is_full_buffer_autogrow()
        {
            const int expectedItemsCount = 3;

            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();

            using var sut = new ListPool<int>(1) { expectedAt0, expectedAt1, expectedAt2 };

            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
            Assert.Equal(expectedAt2, sut[2]);
            Assert.Equal(expectedItemsCount, sut.Count);
        }

        [Fact]
        public void Contains_return_true_when_item_exists()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            int unexpected = s_fixture.Create<int>();

            using var sut = new ListPool<int>(3) { expectedAt0, expectedAt1, expectedAt2 };

            Assert.Contains(expectedAt0, sut);
            Assert.Contains(expectedAt1, sut);
            Assert.Contains(expectedAt2, sut);
            Assert.DoesNotContain(unexpected, sut);
        }

        [Fact]
        public void CopyTo_copy_all_elements_to_target_array()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var sut = new ListPool<int>(3) { expectedAt0, expectedAt1, expectedAt2 };
            int[] array = new int[3];

            sut.CopyTo(array, 0);

            Assert.Equal(sut.Count, array.Length);
            Assert.Contains(expectedAt0, array);
            Assert.Contains(expectedAt1, array);
            Assert.Contains(expectedAt2, array);
        }

        [Fact]
        public void Count_property_is_for_items_Added_count_not_capacity_of_buffer()
        {
            const int listCapacity = 10;
            const int expectedItemsCount = 3;

            using var sut = new ListPool<int>(listCapacity) { 1, 2, 3 };

            Assert.Equal(expectedItemsCount, sut.Count);
        }

        [Fact]
        public void Create_list_and_add_values()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();

            using var sut = new ListPool<int>(3) { expectedAt0, expectedAt1, expectedAt2 };

            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
            Assert.Equal(expectedAt2, sut[2]);
        }

        [Fact]
        public void Create_list_and_add_values_after_clear()
        {
            using var sut =
                new ListPool<int>(3) { s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>() };

            sut.Clear();

            Assert.Empty(sut);
        }

        [Fact]
        public void Create_list_and_add_values_after_remove_by_index()
        {
            const int expectedCountAfterRemove = 2;

            int expectedAt1 = s_fixture.Create<int>();
            using var sut = new ListPool<int>(3) { s_fixture.Create<int>(), expectedAt1, s_fixture.Create<int>() };

            sut.RemoveAt(1);

            Assert.DoesNotContain(expectedAt1, sut);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }

        [Fact]
        public void Create_list_and_add_values_and_after_remove_first()
        {
            const int expectedCountAfterRemove = 2;

            int expectedAt0 = s_fixture.Create<int>();
            using var sut = new ListPool<int>(3) { expectedAt0, s_fixture.Create<int>(), s_fixture.Create<int>() };

            bool wasRemoved = sut.Remove(expectedAt0);

            Assert.True(wasRemoved);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }

        [Fact]
        public void Create_without_parameters_should_add_and_get_items()
        {
            const int expectedItemsCount = 3;

            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var sut = new ListPool<int> { expectedAt0, expectedAt1, expectedAt2 };

            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
            Assert.Equal(expectedAt2, sut[2]);
            Assert.Equal(expectedItemsCount, sut.Count);
        }

        [Fact]
        public void IndexOf_returns_index_of_item()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();

            using var sut = new ListPool<int>(3) { expectedAt0, expectedAt1, expectedAt2 };

            Assert.Equal(0, sut.IndexOf(expectedAt0));
            Assert.Equal(1, sut.IndexOf(expectedAt1));
            Assert.Equal(2, sut.IndexOf(expectedAt2));
        }

        [Fact]
        public void Insert_at_existing_index_move_items_up()
        {
            int[] expectedItems = s_fixture.CreateMany<int>(3).ToArray();
            int expectedItemAt1 = s_fixture.Create<int>();
            int expectedItemsCount = expectedItems.Length + 1;
            using var sut = expectedItems.ToListPool();

            sut.Insert(1, expectedItemAt1);

            Assert.Equal(expectedItemsCount, sut.Count);
            Assert.Equal(expectedItems[0], sut[0]);
            Assert.Equal(expectedItemAt1, sut[1]);
            Assert.Equal(expectedItems[1], sut[2]);
            Assert.Equal(expectedItems[2], sut[3]);
        }

        [Fact]
        public void Insert_at_the_end_add_new_item()
        {
            int expectedAt3 = s_fixture.Create<int>();
            using var sut =
                new ListPool<int>(4) { s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>() };

            sut.Insert(3, expectedAt3);

            Assert.Equal(4, sut.Count);
            Assert.Equal(expectedAt3, sut[3]);
        }

        [Fact]
        public void Set_at_existing_index_update_item()
        {
            int expectedItem = s_fixture.Create<int>();
            int expectedItemsCount = 3;
            using var sut =
                new ListPool<int>(3) { s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>() };

            sut[2] = expectedItem;

            Assert.Equal(expectedItemsCount, sut.Count);
            Assert.Equal(expectedItem, sut[2]);
        }

        [Fact]
        public void ToListPool_from_collection_contains_all_items()
        {
            int[] enumerable = Enumerable.Range(0, 10).ToArray();

            using var sut = enumerable.ToListPool();

            Assert.All(enumerable, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_from_IEnumerable_contains_all_items()
        {
            IEnumerable<int> enumerable = Enumerable.Range(0, 10);

            using var sut = enumerable.ToListPool();

            Assert.All(enumerable, value => sut.Contains(value));
        }
    }
}
