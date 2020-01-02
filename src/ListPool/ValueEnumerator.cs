using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct ValueEnumerator<TSource> : IEnumerator<TSource>
    {
        private readonly TSource[] _source;
        private readonly int _itemsCount;
        private int _index;

        public ValueEnumerator(in TSource[] source, in int itemsCount)
        {
            _source = source;
            _itemsCount = itemsCount;
            _index = -1;
        }

        [MaybeNull]
        public readonly ref readonly TSource Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _source[_index];
        }

        [MaybeNull]
        readonly TSource IEnumerator<TSource>.Current => _source[_index];

        readonly object? IEnumerator.Current => _source[_index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++_index < _itemsCount;
        }

        public void Reset()
        {
            _index = -1;
        }

        public readonly void Dispose()
        {
        }
    }
}
