using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests.ValueListPool
{
    public class ValueListPoolTests : ListPoolTestsBase
    {
        public override void Add_items_when_capacity_is_full_then_buffer_autogrow()
        {
            using var sut = new ValueListPool<int>(128);
            var expectedItems = s_fixture.CreateMany<int>(sut.Capacity * 2).ToList();

            foreach (int expectedItem in expectedItems)
            {
                sut.Add(expectedItem);
            }

            Assert.Equal(expectedItems.Count, sut.Count);
            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }

        public override void Contains_return_true_when_item_exists()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            int unexpected = s_fixture.Create<int>();

            using var sut = new ValueListPool<int>(3);
            sut.Add(expectedAt0);
            sut.Add(expectedAt1);
            sut.Add(expectedAt2);

            Assert.True(sut.Contains(expectedAt0));
            Assert.True(sut.Contains(expectedAt1));
            Assert.True(sut.Contains(expectedAt2));
            Assert.False(sut.Contains(unexpected));
        }


        public override void CopyTo_copy_all_elements_to_target_array()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(expectedAt0);
            sut.Add(expectedAt1);
            sut.Add(expectedAt2);
            int[] array = new int[3];

            sut.CopyTo(array);

            Assert.Equal(sut.Count, array.Length);
            Assert.Contains(expectedAt0, array);
            Assert.Contains(expectedAt1, array);
            Assert.Contains(expectedAt2, array);
        }


        public override void Count_property_is_for_items_Added_count_not_capacity_of_buffer()
        {
            const int listCapacity = 10;
            const int expectedItemsCount = 3;

            using var sut = new ValueListPool<int>(listCapacity);
            sut.Add(1);
            sut.Add(2);
            sut.Add(3);

            Assert.Equal(expectedItemsCount, sut.Count);
        }


        public override void Create_list_and_add_values()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();

            using var sut = new ValueListPool<int>(3);
            sut.Add(expectedAt0);
            sut.Add(expectedAt1);
            sut.Add(expectedAt2);

            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
            Assert.Equal(expectedAt2, sut[2]);
        }


        public override void Create_list_and_add_values_after_clear()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());

            sut.Clear();
            Assert.Equal(0, sut.Count);
            sut.Add(expectedAt0);
            sut.Add(expectedAt1);

            Assert.Equal(2, sut.Count);
            Assert.Equal(expectedAt0, sut[0]);
            Assert.Equal(expectedAt1, sut[1]);
        }

        public override void Get_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            const int index = 2;
            using var sut = new ValueListPool<int>(10);
            sut.Add(s_fixture.Create<int>());
            try
            {
                _ = sut[index];
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }


        public override void Get_item_with_index_bellow_zero_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            int index = -1;
            var sut = new ValueListPool<int>(10);

            try
            {
                _ = sut[index];
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }


        public override void IndexOf_returns_index_of_item()
        {
            int expectedAt0 = s_fixture.Create<int>();
            int expectedAt1 = s_fixture.Create<int>();
            int expectedAt2 = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(expectedAt0);
            sut.Add(expectedAt1);
            sut.Add(expectedAt2);

            Assert.Equal(0, sut.IndexOf(expectedAt0));
            Assert.Equal(1, sut.IndexOf(expectedAt1));
            Assert.Equal(2, sut.IndexOf(expectedAt2));
        }


        public override void Insert_at_existing_index_move_items_up()
        {
            int[] expectedItems = s_fixture.CreateMany<int>(3).ToArray();
            int expectedItemAt1 = s_fixture.Create<int>();
            int expectedItemsCount = expectedItems.Length + 1;
            using var sut = new ValueListPool<int>(expectedItems, ValueListPool<int>.SourceType.UseAsReferenceData);

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
            using var sut = new ValueListPool<int>(4);
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());

            sut.Insert(3, expectedAt3);

            Assert.Equal(4, sut.Count);
            Assert.Equal(expectedAt3, sut[3]);
        }


        public override void Insert_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            bool argumentOutOfRangeException = false;
            const int index = 2;
            using var sut = new ValueListPool<int>(10);
            sut.Add(s_fixture.Create<int>());
            int item = s_fixture.Create<int>();

            try
            {
                sut.Insert(index, item);
            }
            catch (ArgumentOutOfRangeException)
            {
                argumentOutOfRangeException = true;
            }

            Assert.True(argumentOutOfRangeException);
        }


        public override void Insert_item_with_index_bellow_zero_throws_ArgumentOutOfRangeException()
        {
            bool argumentOutOfRangeException = false;
            const int index = -1;
            int item = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(10);

            try
            {
                sut.Insert(index, item);
            }
            catch (ArgumentOutOfRangeException)
            {
                argumentOutOfRangeException = true;
            }

            Assert.True(argumentOutOfRangeException);
        }


        public override void Insert_items_when_capacity_is_full_then_buffer_autogrow()
        {
            using var sut = new ValueListPool<int>(128);
            var expectedItems = s_fixture.CreateMany<int>(sut.Capacity * 2).ToList();
            int index = 0;

            foreach (int expectedItem in expectedItems)
            {
                sut.Insert(index++, expectedItem);
            }

            Assert.Equal(expectedItems.Count, sut.Count);
            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }

        public override void Readonly_property_is_always_false()
        {
            using var sut = new ValueListPool<int>(10);

            Assert.False(sut.IsReadOnly);
        }


        public override void Remove_item_that_doesnt_exists_return_false()
        {
            string item = s_fixture.Create<string>();
            using var sut = new ValueListPool<string>(10);
            sut.Add(s_fixture.Create<string>());

            Assert.False(sut.Remove(item));
            Assert.Equal(1, sut.Count);
        }


        public override void Remove_when_item_exists_remove_item_and_decrease_itemsCount()
        {
            const int expectedCountAfterRemove = 2;
            int expectedAt0 = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(expectedAt0);
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());

            bool wasRemoved = sut.Remove(expectedAt0);

            Assert.True(wasRemoved);
            Assert.False(sut.Contains(expectedAt0));
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }


        public override void Remove_when_item_is_null_return_false()
        {
            string item = null;
            using var sut = new ValueListPool<string>(10);

            Assert.False(sut.Remove(item));
        }


        public override void RemoveAt_when_item_exists_remove_item_and_decrease_itemsCount()
        {
            const int expectedCountAfterRemove = 2;
            int expectedAt1 = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(s_fixture.Create<int>());
            sut.Add(expectedAt1);
            sut.Add(s_fixture.Create<int>());

            sut.RemoveAt(1);

            Assert.False(sut.Contains(expectedAt1));
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }


        public override void RemoveAt_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            const int index = 2;
            using var sut = new ValueListPool<int>(10);
            sut.Add(s_fixture.Create<int>());

            try
            {
                sut.RemoveAt(index);
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }


        public override void RemoveAt_with_index_bellow_zero_throws_ArgumentOutOfRangeException()
        {
            bool argumentOutOfRangeException = false;
            const int index = -1;
            using var sut = new ValueListPool<int>(10);

            try
            {
                sut.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                argumentOutOfRangeException = true;
            }

            Assert.True(argumentOutOfRangeException);
        }


        public override void RemoveAt_with_index_zero_when_not_item_added_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            const int index = 0;
            using var sut = new ValueListPool<int>(10);

            try
            {
                sut.RemoveAt(index);
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }


        public override void Set_at_existing_index_update_item()
        {
            const int expectedItemsCount = 3;
            int expectedItem = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(3);
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());
            sut.Add(s_fixture.Create<int>());

            sut[2] = expectedItem;

            Assert.Equal(expectedItemsCount, sut.Count);
            Assert.Equal(expectedItem, sut[2]);
        }


        public override void Set_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            const int index = 2;
            using var sut = new ValueListPool<int>(10);
            sut.Add(s_fixture.Create<int>());
            int item = s_fixture.Create<int>();

            try
            {
                sut[index] = item;
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }


        public override void Set_item_with_index_bellow_zero_throws_IndexOutOfRangeException()
        {
            bool indexOutOfRangeExceptionThrown = false;
            const int index = -1;
            int item = s_fixture.Create<int>();
            var sut = new ValueListPool<int>(10);

            try
            {
                sut[index] = item;
            }
            catch (IndexOutOfRangeException)
            {
                indexOutOfRangeExceptionThrown = true;
            }

            Assert.True(indexOutOfRangeExceptionThrown);
        }

        [Fact]
        public void AddRange_adds_all_items()
        {
            int[] expectedValues = Enumerable.Range(0, 10).ToArray();
            int expectedItem0 = s_fixture.Create<int>();
            int expectedItem1 = s_fixture.Create<int>();
            int expectedItem2 = s_fixture.Create<int>();
            int expectedItemAtTheEnd = s_fixture.Create<int>();
            int expectedCount = expectedValues.Length + 4;
            using var sut = new ValueListPool<int>(20);
            sut.Add(expectedItem0);
            sut.Add(expectedItem1);
            sut.Add(expectedItem2);

            sut.AddRange(expectedValues);
            sut.Add(expectedItemAtTheEnd);

            Assert.Equal(expectedCount, sut.Count);
            Assert.Equal(expectedItem0, sut[0]);
            Assert.Equal(expectedItem1, sut[1]);
            Assert.Equal(expectedItem2, sut[2]);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }

            Assert.Equal(expectedItemAtTheEnd, sut[13]);
        }

        [Fact]
        public void AddRange_from_array_adds_all_items()
        {
            int[] expectedValues = Enumerable.Range(0, 10).ToArray();
            using var sut = new ValueListPool<int>(10);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AddRange_from_array_bigger_than_capacity_then_it_grows_and_add_items()
        {
            int[] expectedValues = Enumerable.Range(0, 1000).ToArray();
            using var sut = new ValueListPool<int>(128);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AddRange_from_ReadOnlySpan_adds_all_items()
        {
            ReadOnlySpan<int> expectedValues = Enumerable.Range(0, 10).ToArray();
            using var sut = new ValueListPool<int>(10);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AddRange_from_ReadOnlySpan_bigger_than_capacity_then_it_grows_and_add_items()
        {
            ReadOnlySpan<int> expectedValues = Enumerable.Range(0, 1000).ToArray();
            using var sut = new ValueListPool<int>(64);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AddRange_from_span_adds_all_items()
        {
            Span<int> expectedValues = Enumerable.Range(0, 10).ToArray();
            using var sut = new ValueListPool<int>(10);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AddRange_from_span_bigger_than_capacity_then_it_grows_and_add_items()
        {
            Span<int> expectedValues = Enumerable.Range(0, 1000).ToArray();
            using var sut = new ValueListPool<int>(64);

            sut.AddRange(expectedValues);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AsSpan_returns_span_for_added_items()
        {
            int[] expectedValues = s_fixture.Create<int[]>();
            using var listPool = new ValueListPool<int>(expectedValues, ValueListPool<int>.SourceType.Copy);

            Span<int> sut = listPool.AsSpan();

            Assert.Equal(expectedValues.Length, sut.Length);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void AsSpan_when_not_items_Added_returns_empty_span()
        {
            using var listPool = new ValueListPool<int>(10);

            Span<int> sut = listPool.AsSpan();

            Assert.Equal(0, sut.Length);
        }

        [Fact]
        public void Create_large_ValueListPool_from_copy()
        {
            Span<int> values = Enumerable.Range(0, 1000).ToArray();

            using var sut = new ValueListPool<int>(values, ValueListPool<int>.SourceType.Copy);

            IEnumerable<int> expectedValues = values.ToArray();
            Assert.Equal(expectedValues.Count(), sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void Create_list_by_passing_another_without_items_set_minimum_capacity()
        {
            int[] emptyList = new int[0];

            using ValueListPool<int> sut = new ValueListPool<int>(emptyList, ValueListPool<int>.SourceType.Copy);

            Assert.Equal(16, sut.Capacity);
        }

        [Fact]
        public void Create_ValueListPool_by_copying__from_large_array_it_uses_capacity_equal_or_bigger_than_collection()
        {
            int[] expectedValues = Enumerable.Range(0, 10).ToArray();

            using var sut = new ValueListPool<int>(expectedValues, ValueListPool<int>.SourceType.Copy);

            Assert.Equal(expectedValues.Length, sut.Count);
            Assert.True(sut.Capacity >= expectedValues.Length);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void Create_ValueListPool_by_copying_from_small_array_it_uses_minimum_capacity()
        {
            int[] expectedValues = Enumerable.Range(0, 10).ToArray();

            using var sut = new ValueListPool<int>(expectedValues, ValueListPool<int>.SourceType.Copy);

            Assert.Equal(expectedValues.Length, sut.Count);
            Assert.Equal(16, sut.Capacity);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void Create_ValueListPool_from_empty_InitialBuffer_and_add_new_value_it_grows_using_minimum_capacity()
        {
            Span<int> emptyBuffer = stackalloc int[0];
            int expectedItem = s_fixture.Create<int>();
            using var sut = new ValueListPool<int>(emptyBuffer, ValueListPool<int>.SourceType.UseAsInitialBuffer);

            sut.Add(expectedItem);

            Assert.Equal(16, sut.Capacity);
            Assert.Equal(1, sut.Count);
            Assert.Equal(expectedItem, sut[0]);
        }

        [Fact]
        public void Create_ValueListPool_from_small_InitialBuffer_and_when_it_grows_it_use_minimum_capacity()
        {
            Span<int> emptyBuffer = stackalloc int[1];
            int expectedItemAt0 = s_fixture.Create<int>();
            int expectedItemAt1 = s_fixture.Create<int>();
            int expectedItemAt2 = s_fixture.Create<int>();

            using var sut = new ValueListPool<int>(emptyBuffer, ValueListPool<int>.SourceType.UseAsInitialBuffer);
            sut.Add(expectedItemAt0);
            sut.Add(expectedItemAt1);
            sut.Add(expectedItemAt2);

            Assert.Equal(16, sut.Capacity);
            Assert.Equal(3, sut.Count);
            Assert.Equal(expectedItemAt0, sut[0]);
            Assert.Equal(expectedItemAt1, sut[1]);
            Assert.Equal(expectedItemAt2, sut[2]);
        }

        [Fact]
        public void Create_ValueListPool_from_stackalloc()
        {
            Span<int> expectedValues = stackalloc int[5] {1, 2, 3, 4, 5};

            using var sut = new ValueListPool<int>(expectedValues, ValueListPool<int>.SourceType.UseAsReferenceData);

            Assert.Equal(expectedValues.Length, sut.Count);
            foreach (int expectedValue in expectedValues)
            {
                Assert.True(sut.Contains(expectedValue));
            }
        }

        [Fact]
        public void Create_ValueListPool_using_stackalloc_buffer()
        {
            Span<int> expectedValues = stackalloc int[5] {1, 2, 3, 4, 5};

            using var sut = new ValueListPool<int>(expectedValues, ValueListPool<int>.SourceType.UseAsInitialBuffer);

            Assert.Equal(0, sut.Count);
        }

        [Fact]
        public void Create_ValueListPool_using_stackalloc_buffer_and_grow_using_pooled_array()
        {
            using var sut =
                new ValueListPool<int>(stackalloc int[50], ValueListPool<int>.SourceType.UseAsInitialBuffer);

            for (int i = 0; i < 100; i++)
            {
                sut.Add(i);
            }

            Assert.Equal(100, sut.Count);
            for (int i = 0; i < 100; i++)
            {
                Assert.True(sut.Contains(i));
            }
        }
    }
}
