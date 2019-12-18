using System;
using System.Linq;
using Xunit;

namespace ListPool.UnitTests
{
    public class ListPoolTests
    {
        [Fact]
        public void Create_list_and_AddValues()
        {
            int expectedAtFirst = 5;
            int expectedAtSecond = 7;
            int expectedAtThird = 10;

            using var sut = new ListPool<int>(3);
            sut.Add(expectedAtFirst);
            sut.Add(expectedAtSecond);
            sut.Add(expectedAtThird);

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);
        }

        [Fact]
        public void Enumerate_added_items_and_ignore_others()
        {
            int listCapacity = 10;
            int expectedCount = 3;

            using var sut = new ListPool<int>(listCapacity);
            sut.Add(1);
            sut.Add(2);
            sut.Add(3);

            Assert.Equal(expectedCount, sut.Count());
        }

        [Fact]
        public void ListPool_should_autogrow()
        {
            int expectedAtFirst = 5;
            int expectedAtSecond = 7;
            int expectedAtThird = 10;
            int expectedSize = 3;

            using var sut = new ListPool<int>(1);
            sut.Add(expectedAtFirst);
            sut.Add(expectedAtSecond);
            sut.Add(expectedAtThird);

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);
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