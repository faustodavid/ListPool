using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public static class ListPoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ListPool<T> ToListPool<T>(this IEnumerable<T> source) => new ListPool<T>(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ListPool<T> ToListPool<T>(this Span<T> source) => new ListPool<T>(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ListPool<T> ToListPool<T>(this ReadOnlySpan<T> source) => new ListPool<T>(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueListPool<T> ToValueListPool<T>(this Span<T> source,
                                                          ValueListPool<T>.SourceType sourceType = ValueListPool<T>.SourceType.UseAsReferenceData) where T : IEquatable<T>
            => new ValueListPool<T>(source, sourceType);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueListPool<T> ToValueListPool<T>(this T[] source, ValueListPool<T>.SourceType sourceType = ValueListPool<T>.SourceType.UseAsReferenceData) where T : IEquatable<T>
            => new ValueListPool<T>(source, sourceType);
    }
}
