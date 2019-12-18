using System;
using System.Collections.Generic;

namespace ListPool
{
    public static class ListPoolExtensions
    {
        public static ListPool<TSource> ToListPool<TSource>(this IEnumerable<TSource> source) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new ListPool<TSource>(source);
        }
    }
}