using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ListPool.Netstandard2_0.UnitTests
{
    public class ListPoolExtensionsTests
    {
        [Fact]
        public void ToListPool_from_array_contains_all_items()
        {
            int[] expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToListPool();

            Assert.All(expectedItems, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_from_collection_contains_all_items()
        {
            ICollection<int> expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToListPool();

            Assert.All(expectedItems, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_from_IEnumerable_contains_all_items()
        {
            IEnumerable<int> expectedItems = Enumerable.Range(0, 10);

            using var sut = expectedItems.ToListPool();

            Assert.All(expectedItems, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_from_ReadOnlySpan_contains_all_items()
        {
            ReadOnlySpan<int> expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToListPool();

            foreach (int expectedItem in expectedItems)
            {
                Assert.Contains(expectedItem, sut);
            }
        }


        [Fact]
        public void ToListPool_from_Span_contains_all_items()
        {
            ReadOnlySpan<int> expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToListPool();

            foreach (int expectedItem in expectedItems)
            {
                Assert.Contains(expectedItem, sut);
            }
        }

        [Fact]
        public void ToListPool_when_source_is_null_throw_ArgumentNullException()
        {
            IEnumerable<int> source = null;
            string expectedName = nameof(source);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => source.ToListPool());

            Assert.Contains(expectedName, exception.Message);
        }

        [Fact]
        public void ToValueListPool_from_array_as_reference_contains_all_items()
        {
            int[] expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToValueListPool();

            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }

        [Fact]
        public void ToValueListPool_from_array_copy_contains_all_items()
        {
            int[] expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToValueListPool(ValueListPool<int>.SourceType.Copy);

            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }

        [Fact]
        public void ToValueListPool_from_span__copy_contains_all_items()
        {
            Span<int> expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToValueListPool(ValueListPool<int>.SourceType.Copy);

            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }


        [Fact]
        public void ToValueListPool_from_span_contains_all_items()
        {
            Span<int> expectedItems = Enumerable.Range(0, 10).ToArray();

            using var sut = expectedItems.ToValueListPool();

            foreach (int expectedItem in expectedItems)
            {
                Assert.True(sut.Contains(expectedItem));
            }
        }
    }
}
