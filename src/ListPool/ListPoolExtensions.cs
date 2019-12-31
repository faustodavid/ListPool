using System;
using System.Collections.Generic;

namespace ListPool
{
    public static class ListPoolExtensions
    {
        public static ListPool<TSource> ToListPool<TSource>(this IEnumerable<TSource> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return new ListPool<TSource>(source);
        }

        public static ListPoolValue<TSource> ToListPoolValue<TSource>(this IEnumerable<TSource> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return new ListPoolValue<TSource>(source);
        }
    }
}
