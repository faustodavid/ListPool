using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ListPool.UnitTests
{
    public class ListPoolTests
    {
        [Fact]
        public void Create_list_and_add_values()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);
        }

        [Fact]
        public void Create_ListPool_from_IEnumerable()
        {
            var sut = Enumerable.Range(0, 10).Select(e => e).ToListPool();
        }

        [Fact]
        public void Enumerate_added_items_and_ignore_others()
        {
            const int listCapacity = 10;
            const int expectedCount = 3;

            using var sut = new ListPool<int>(listCapacity)
            {
                1,
                2,
                3
            };

            Assert.Equal(expectedCount, sut.Count);
        }

        [Fact]
        public void ListPool_should_autogrow()
        {
            int expectedAtFirst = 5;
            int expectedAtSecond = 7;
            int expectedAtThird = 10;

            using var sut = new ListPool<int>(1)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);
        }

        [Fact]
        public void Create_list_and_add_values_after_remove()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;
            const int expectedCountAfterRemove = 2;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            Assert.True(sut.Remove(expectedAtFirst));
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }

        [Fact]
        public void Create_list_and_add_values_after_remove_by_index()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;
            const int expectedCountAfterRemove = 2;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            sut.RemoveAt(1);
            var actualResult = sut.Contains(expectedAtSecond);

            Assert.False(actualResult);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }

        [Fact]
        public void Create_list_and_add_values_after_clear()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            sut.Clear();
            var actualFirst = sut.Contains(expectedAtFirst);
            var actualSecond = sut.Contains(expectedAtSecond);
            var actualThird = sut.Contains(expectedAtThird);

            Assert.False(actualFirst);
            Assert.False(actualSecond);
            Assert.False(actualThird);
            Assert.Empty(sut);
        }

        [Fact]
        public void Create_list_and_add_values_and_call_contains()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            var actualFirst = sut.Contains(expectedAtFirst);
            var actualSecond = sut.Contains(expectedAtSecond);
            var actualThird = sut.Contains(expectedAtThird);

            Assert.NotEmpty(sut);
            Assert.True(actualFirst);
            Assert.True(actualSecond);
            Assert.True(actualThird);
        }

        [Fact]
        public void Create_list_and_add_values_and_call_copy_to()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            var actualArray = new int[3];
            sut.CopyTo(actualArray, 0);

            Assert.Equal(sut.Count, actualArray.Length);

            for (var i = 0; i < sut.Count; i++) 
            {
                Assert.Equal(sut[i], actualArray[i]);
            }
        }

        [Fact]
        public void Create_list_and_add_values_and_call_index_of()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;

            using var sut = new ListPool<int>(3)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            Assert.Equal(0, sut.IndexOf(expectedAtFirst));
            Assert.Equal(1, sut.IndexOf(expectedAtSecond));
            Assert.Equal(2, sut.IndexOf(expectedAtThird));
        }

        [Fact]
        public void Create_list_and_add_values_and_call_insert()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;
            const int expectedAtForth= 15;

            var sut = new ListPool<int>(10)
            {
                expectedAtFirst,
                expectedAtSecond,
                expectedAtThird
            };

            sut.Insert(3, expectedAtForth);

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);
            Assert.Equal(expectedAtForth, sut[3]);
        }

        [Fact]
        public void ToListPool()
        {
            var source = new[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};

            var sourceAsListPool = source.ToListPool();

            Assert.Equal(source[0], sourceAsListPool[0]);
            Assert.Equal(source[1], sourceAsListPool[1]);
        }
    }
}