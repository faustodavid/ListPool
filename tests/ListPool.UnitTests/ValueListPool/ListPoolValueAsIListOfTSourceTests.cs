using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests.ValueListPool
{
    public class ListPoolValueAsIListOfTSourceTests : ListPoolTestsBase
    {
        public override void Add_items_when_capacity_is_full_then_buffer_autogrow()
        {
            using var listPool = new ValueListPool<int>(128);
            IList<int> sut = listPool;
            var expectedItems = s_fixture.CreateMany<int>(listPool.Capacity * 2).ToList();

            foreach (int expectedItem in expectedItems)
            {
                sut.Add(expectedItem);
            }

            Assert.Equal(expectedItems.Count, sut.Count);
            Assert.True(expectedItems.All(expectedItem => sut.Contains(expectedItem)));
        }

        public override void Contains_return_true_when_item_exists()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            int unexpected = s_fixture.Create<int>();

            using var listPool = new ValueListPool<int>(3) {expectedAt0, expectedAt1, expectedAt2};
            IList<int> sut = listPool;

            Assert.Contains(expectedAt0, sut);
            Assert.Contains(expectedAt1, sut);
            Assert.Contains(expectedAt2, sut);
            Assert.DoesNotContain(unexpected, sut);
        }


        public override void CopyTo_copy_all_elements_to_target_array()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var listPool = new ValueListPool<int>(3) {expectedAt0, expectedAt1, expectedAt2};
            IList<int> sut = listPool;
            int[] array = new int[3];

            sut.CopyTo(array, 0);

            Assert.Equal(sut.Count, array.Length);
            Assert.Contains(expectedAt0, array);
            Assert.Contains(expectedAt1, array);
            Assert.Contains(expectedAt2, array);
        }


        public override void Count_property_is_for_items_Added_count_not_capacity_of_buffer()
        {
            const int listCapacity = 10;
            const int expectedItemsCount = 3;

            using var listPool = new ValueListPool<int>(listCapacity) {1, 2, 3};
            IList<int> sut = listPool;

            Assert.Equal(expectedItemsCount, sut.Count);
        }


        public override void Create_list_and_add_values()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();

            using var listPool = new ValueListPool<int>(3) {expectedAt0, expectedAt1, expectedAt2};
            IList<int> sut = listPool;

            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
            Assert.Equal(expectedAt2, sut[2]);
        }


        public override void Create_list_and_add_values_after_clear()
        {
            using var listPool =
                new ValueListPool<int>(3) {s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()};
            IList<int> sut = listPool;

            sut.Clear();

            Assert.Empty(sut);
        }

        public override void Get_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            const int index = 2;
            using var listPool = new ValueListPool<int>(10) {s_fixture.Create<int>()};
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut[index]);
        }


        public override void Get_item_with_index_bellow_zero_throws_IndexOutOfRangeException()
        {
            int index = -1;
            var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut[index]);
        }


        public override void IndexOf_returns_index_of_item()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var listPool = new ValueListPool<int>(3) {expectedAt0, expectedAt1, expectedAt2};
            IList<int> sut = listPool;

            Assert.Equal(0, sut.IndexOf(expectedAt0));
            Assert.Equal(1, sut.IndexOf(expectedAt1));
            Assert.Equal(2, sut.IndexOf(expectedAt2));
        }


        public override void Insert_at_existing_index_move_items_up()
        {
            int[] expectedItems = s_fixture.CreateMany<int>(3).ToArray();
            int expectedItemAt1 = s_fixture.Create<int>();
            int expectedItemsCount = expectedItems.Length + 1;
            using var listPool = expectedItems.ToListPool();
            IList<int> sut = listPool;

            sut.Insert(1, expectedItemAt1);

            Assert.Equal(expectedItemsCount, sut.Count);
            Assert.Equal(expectedItems[0], sut[0]);
            Assert.Equal(expectedItemAt1, sut[1]);
            Assert.Equal(expectedItems[1], sut[2]);
            Assert.Equal(expectedItems[2], sut[3]);
        }


        public override void Insert_at_the_end_add_new_item()
        {
            int expectedAt3 = s_fixture.Create<int>();
            using var listPool =
                new ValueListPool<int>(4) {s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()};
            IList<int> sut = listPool;

            sut.Insert(3, expectedAt3);

            Assert.Equal(4, sut.Count);
            Assert.Equal(expectedAt3, sut[3]);
        }


        public override void Insert_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            const int index = 2;
            using var listPool = new ValueListPool<int>(10) {s_fixture.Create<int>()};
            IList<int> sut = listPool;
            int item = s_fixture.Create<int>();

            Assert.Throws<IndexOutOfRangeException>(() => sut.Insert(index, item));
        }


        public override void Insert_item_with_index_bellow_zero_throws_ArgumentOutOfRangeException()
        {
            const int index = -1;
            int item = s_fixture.Create<int>();
            using var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut.Insert(index, item));
        }


        public override void Insert_items_when_capacity_is_full_then_buffer_autogrow()
        {
            using var listPool = new ValueListPool<int>(128);
            IList<int> sut = listPool;
            var expectedItems = s_fixture.CreateMany<int>(listPool.Capacity * 2).ToList();
            int index = 0;

            foreach (int expectedItem in expectedItems)
            {
                sut.Insert(index++, expectedItem);
            }

            Assert.Equal(expectedItems.Count, sut.Count);
            Assert.True(expectedItems.All(expectedItem => sut.Contains(expectedItem)));
        }


        public override void Readonly_property_is_always_false()
        {
            using var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.False(sut.IsReadOnly);
        }


        public override void Remove_item_that_doesnt_exists_return_false()
        {
            string item = s_fixture.Create<string>();
            using var listPool = new ValueListPool<string>(10) {s_fixture.Create<string>()};
            IList<string> sut = listPool;

            Assert.False(sut.Remove(item));
            Assert.Single(sut);
        }


        public override void Remove_when_item_exists_remove_item_and_decrease_itemsCount()
        {
            const int expectedCountAfterRemove = 2;
            int expectedAt0 = s_fixture.Create<int>();
            using var listPool =
                new ValueListPool<int>(3) {expectedAt0, s_fixture.Create<int>(), s_fixture.Create<int>()};
            IList<int> sut = listPool;

            bool wasRemoved = sut.Remove(expectedAt0);

            Assert.True(wasRemoved);
            Assert.DoesNotContain(expectedAt0, sut);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }


        public override void Remove_when_item_is_null_return_false()
        {
            string item = null;
            using var listPool = new ValueListPool<string>(10);
            IList<string> sut = listPool;

            Assert.False(sut.Remove(item));
        }


        public override void RemoveAt_when_item_exists_remove_item_and_decrease_itemsCount()
        {
            const int expectedCountAfterRemove = 2;
            int expectedAt1 = s_fixture.Create<int>();
            using var listPool =
                new ValueListPool<int>(3) {s_fixture.Create<int>(), expectedAt1, s_fixture.Create<int>()};
            IList<int> sut = listPool;

            sut.RemoveAt(1);

            Assert.DoesNotContain(expectedAt1, sut);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }


        public override void RemoveAt_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            const int index = 2;
            using var listPool = new ValueListPool<int>(10) {s_fixture.Create<int>()};
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut.RemoveAt(index));
        }


        public override void RemoveAt_with_index_bellow_zero_throws_ArgumentOutOfRangeException()
        {
            const int index = -1;
            using var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut.RemoveAt(index));
        }


        public override void RemoveAt_with_index_zero_when_not_item_added_throws_IndexOutOfRangeException()
        {
            const int index = 0;
            using var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut.RemoveAt(index));
        }


        public override void Set_at_existing_index_update_item()
        {
            const int expectedItemsCount = 3;
            int expectedItem = s_fixture.Create<int>();
            using var listPool =
                new ValueListPool<int>(3) {s_fixture.Create<int>(), s_fixture.Create<int>(), s_fixture.Create<int>()};
            IList<int> sut = listPool;

            sut[2] = expectedItem;

            Assert.Equal(expectedItemsCount, sut.Count);
            Assert.Equal(expectedItem, sut[2]);
        }


        public override void Set_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            const int index = 2;
            using var listPool = new ValueListPool<int>(10) {s_fixture.Create<int>()};
            IList<int> sut = listPool;
            int item = s_fixture.Create<int>();

            Assert.Throws<IndexOutOfRangeException>(() => sut[index] = item);
        }


        public override void Set_item_with_index_bellow_zero_throws_IndexOutOfRangeException()
        {
            const int index = -1;
            int item = s_fixture.Create<int>();
            var listPool = new ValueListPool<int>(10);
            IList<int> sut = listPool;

            Assert.Throws<IndexOutOfRangeException>(() => sut[index] = item);
        }
    }
}
