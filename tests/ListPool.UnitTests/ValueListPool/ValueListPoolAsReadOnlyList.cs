using System;
using System.Collections.Generic;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests.ValueListPool
{
    public class ValueListPoolAsReadOnlyList
    {
        private static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void Get_existing_item_by_index()
        {
            int expectedItem = s_fixture.Create<int>();
            using var list = new ValueListPool<int>(10) {  s_fixture.Create<int>(), expectedItem,  s_fixture.Create<int>()};

            IReadOnlyList<int> sut = list;

            Assert.Equal(expectedItem, sut[1]);
        }

        [Fact]
        public void Get_item_with_index_above_itemsCount_throws_IndexOutOfRangeException()
        {
            const int index = 2;
            using var list = new ValueListPool<int>(10) {s_fixture.Create<int>()};

            IReadOnlyList<int> sut = list;

            Assert.Throws<IndexOutOfRangeException>(() => sut[index]);
        }

        [Fact]
        public void Get_item_with_index_bellow_zero_throws_IndexOutOfRangeException()
        {
            const int index = -1;
            var list = new ValueListPool<int>(10);

            IReadOnlyList<int> sut = list;

            Assert.Throws<IndexOutOfRangeException>(() => sut[index]);
        }
    }
}
