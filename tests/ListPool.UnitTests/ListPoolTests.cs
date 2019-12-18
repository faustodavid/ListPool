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
            const int listCapacity = 10;
            const int expectedCount = 3;

            using var sut = ListPool<int>.Rent(listCapacity);
            sut.Add(1);
            sut.Add(2);
            sut.Add(3);

            Assert.Equal(expectedCount, sut.Count);
        }

        [Fact]
        public void Create_list_and_add_values_after_remove()
        {
            const int expectedAtFirst = 5;
            const int expectedAtSecond = 7;
            const int expectedAtThird = 10;
            const int expectedCountAfterRemove = 2;

            using var sut = ListPool<int>.Rent(3);
            sut.Add(expectedAtFirst);
            sut.Add(expectedAtSecond);
            sut.Add(expectedAtThird);

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

            using var sut = ListPool<int>.Rent(3);
            sut.Add(expectedAtFirst);
            sut.Add(expectedAtSecond);
            sut.Add(expectedAtThird);

            Assert.Equal(expectedAtFirst, sut[0]);
            Assert.Equal(expectedAtSecond, sut[1]);
            Assert.Equal(expectedAtThird, sut[2]);

            sut.RemoveAt(1);
            var actualResult = sut.Contains(expectedAtSecond);

            Assert.False(actualResult);
            Assert.Equal(expectedCountAfterRemove, sut.Count);
        }
    }
}