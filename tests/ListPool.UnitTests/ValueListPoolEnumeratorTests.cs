using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests
{
    public class ValueListPoolEnumeratorTests
    {
        private static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void GetEnumerator_Enumerate_All_Items()
        {
            int[] expectedItems = s_fixture.CreateMany<int>(10).ToArray();
            using ValueListPool<int> listPool = new ValueListPool<int>(expectedItems);
            using ValueListPool<int>.Enumerator sut = listPool.GetEnumerator();
            List<int> actualItems = new List<int>(expectedItems.Length);

            while (sut.MoveNext())
            {
                actualItems.Add(sut.Current);
            }

            Assert.Equal(expectedItems.Length, actualItems.Count);
            Assert.Contains(expectedItems, expectedItem => actualItems.Contains(expectedItem));
        }

        [Fact]
        public void Current_is_updated_in_each_iteration()
        {
            string[] items = s_fixture.CreateMany<string>(10).ToArray();
            IEnumerator expectedEnumerator = items.GetEnumerator();
            var sut = new ValueListPool<string>.Enumerator(items, items.Length);

            while (expectedEnumerator.MoveNext())
            {
                Assert.True(sut.MoveNext());
                Assert.Equal(expectedEnumerator.Current, sut.Current);
            }
        }

        [Fact]
        public void Current_is_updated_in_each_iteration_using_IEnumerator()
        {
            string[] items = s_fixture.CreateMany<string>(10).ToArray();
            IEnumerator expectedEnumerator = items.GetEnumerator();
            IEnumerator sut = new ValueListPool<string>.Enumerator(items, items.Length);

            while (expectedEnumerator.MoveNext())
            {
                Assert.True(sut.MoveNext());
                Assert.Equal(expectedEnumerator.Current, sut.Current);
            }
        }

        [Fact]
        public void Reset_allows_enumerator_to_be_enumerate_again()
        {
            string[] items = s_fixture.CreateMany<string>(10).ToArray();
            IEnumerator expectedEnumerator = items.GetEnumerator();
            var sut = new ValueListPool<string>.Enumerator(items, items.Length);

            while (expectedEnumerator.MoveNext())
            {
                Assert.True(sut.MoveNext());
                Assert.Equal(expectedEnumerator.Current, sut.Current);
            }

            Assert.False(sut.MoveNext());
            sut.Reset();
            expectedEnumerator.Reset();
            while (expectedEnumerator.MoveNext())
            {
                Assert.True(sut.MoveNext());
                Assert.Equal(expectedEnumerator.Current, sut.Current);
            }
        }
    }
}
