using System.Collections;
using System.Linq;
using AutoFixture;
using Xunit;

namespace ListPool.UnitTests
{
    public class EnumeratorTests
    {
        private static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void Current_is_updated_in_each_iteration()
        {
            string[] items = s_fixture.CreateMany<string>(10).ToArray();
            IEnumerator expectedEnumerator = items.GetEnumerator();
            var sut = new Enumerator<string>(items, items.Length);

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
            IEnumerator sut = new Enumerator<string>(items, items.Length);

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
            var sut = new Enumerator<string>(items, items.Length);

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
