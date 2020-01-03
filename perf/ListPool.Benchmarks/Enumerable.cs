using System.Collections;
using System.Collections.Generic;

namespace ListPool.Benchmarks
{
    public class Enumerable<TSource>: IEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;

        public Enumerable(IEnumerable<TSource> source) => _source = source;

        public IEnumerator<TSource> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();
    }
}