using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace ListPool
{
    public struct ValueEnumerableWrapper<TSource>
        : IValueEnumerable<TSource, ValueEnumerableWrapper<TSource>.Enumerator>
    {
        readonly IEnumerable<TSource> source;

        internal ValueEnumerableWrapper(IEnumerable<TSource> source)
        {
            this.source = source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator() => new Enumerator(source);

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => new Enumerator(source);
        readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(source);

        public readonly struct Enumerator
            : IEnumerator<TSource>
        {
            readonly IEnumerator<TSource> enumerator;

            internal Enumerator(IEnumerable<TSource> enumerable)
            {
                enumerator = enumerable.GetEnumerator();
            }

            [MaybeNull]
            public readonly TSource Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => enumerator.Current;
            }

            readonly object? IEnumerator.Current => enumerator.Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MoveNext() => enumerator.MoveNext();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void Reset() => enumerator.Reset();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void Dispose() => enumerator.Dispose();
        }
    }
}