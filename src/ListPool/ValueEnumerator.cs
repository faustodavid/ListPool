using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct ValueEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _source;
        private readonly int _itemsCount;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueEnumerator(T[] source, int itemsCount)
        {
            _source = source;
            _itemsCount = itemsCount;
            _index = -1;
        }

        [MaybeNull]
        public readonly ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _source[_index];
        }

        [MaybeNull]
        readonly T IEnumerator<T>.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source[_index];
        }

        [MaybeNull]
        readonly object? IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_index < _itemsCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _index = -1;
        }

        public readonly void Dispose()
        {
        }
    }
}
