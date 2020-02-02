using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public static class ListPoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ListPool<TSource> ToListPool<TSource>(this IEnumerable<TSource> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return new ListPool<TSource>(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueListPool<TSource> ToValueListPool<TSource>(this IEnumerable<TSource> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return new ValueListPool<TSource>(source);
        }
    }
}
