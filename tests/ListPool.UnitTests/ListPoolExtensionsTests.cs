using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ListPool.UnitTests
{
    public class ListPoolExtensionsTests
    {
        [Fact]
        public void ToListPool_from_collection_contains_all_items()
        {
            int[] enumerable = Enumerable.Range(0, 10).ToArray();

            using var sut = enumerable.ToListPool();

            Assert.All(enumerable, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_from_IEnumerable_contains_all_items()
        {
            IEnumerable<int> enumerable = Enumerable.Range(0, 10);

            using var sut = enumerable.ToListPool();

            Assert.All(enumerable, value => sut.Contains(value));
        }

        [Fact]
        public void ToListPool_when_source_is_null_throw_ArgumentNullException()
        {
            IEnumerable<int> source = null;
            string expectedName = nameof(source);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => source.ToListPool());

            Assert.Contains(expectedName, exception.Message);
        }
    }
}
