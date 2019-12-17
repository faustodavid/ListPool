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

            using var sut = ListPool<int>.Rent(3);
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

            using var sut = ListPool<int>.Rent(listCapacity);
            sut.Add(1);
            sut.Add(2);
            sut.Add(3);

            Assert.Equal(expectedCount, sut.Count());
        }
    }
}