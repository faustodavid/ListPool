using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct Enumerator<TSource> : IEnumerator<TSource>
    {
        private readonly TSource[] source;
        private readonly int itemsCount;
        private int index;

        public Enumerator(in TSource[] source, in int itemsCount)
        {
            this.source = source;
            this.itemsCount = itemsCount;
            index = -1;
        }

        [MaybeNull]
        public readonly ref readonly TSource Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref source[index];
        }

        [MaybeNull]
        readonly TSource IEnumerator<TSource>.Current => source[index];

        readonly object? IEnumerator.Current => source[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++index < itemsCount;
        }

        public void Reset()
        {
            index = -1;
        }

        public readonly void Dispose()
        {
        }
    }
}