using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct ListPool<TSource> : IDisposable, IValueEnumerable<TSource>, IList<TSource>
    {
        private readonly ArrayPool<TSource> _arrayPool;
        private TSource[] _buffer;
        private int _itemsCount;
        private const int _minimumCapacity = 100;

        public readonly int Count => _itemsCount;
        public readonly int Capacity => _buffer.Length;
        public bool IsReadOnly { get; }

        public ListPool(int length)
        {
            _arrayPool = ArrayPool<TSource>.Shared;
            _buffer = _arrayPool.Rent(length);
            _itemsCount = 0;

            IsReadOnly = false;
        }

        public ListPool(IEnumerable<TSource> source)
        {
            _arrayPool = ArrayPool<TSource>.Shared;
            IsReadOnly = false;

            if (source is ICollection collection)
            {
                _buffer = _arrayPool.Rent(collection.Count);
                _itemsCount = collection.Count;

                collection.CopyTo(_buffer, 0);
            }
            else
            {
                _buffer = _arrayPool.Rent(_minimumCapacity);
                _itemsCount = 0;

                foreach (var item in source)
                {
                    Add(item);
                }
            }
        }

        public void Add(TSource item)
        {
            if (_itemsCount >= _buffer.Length) GrowBuffer();

            _buffer[_itemsCount] = item;
            _itemsCount++;
        }

        public void Clear() => _itemsCount = 0;

        public readonly bool Contains(TSource item) => _itemsCount > 0 && IndexOf(item) > -1;

        public readonly int IndexOf(TSource item) => Array.IndexOf(_buffer, item, 0, _itemsCount);

        public readonly void CopyTo(TSource[] array, int arrayIndex) => Array.Copy(_buffer, 0, array, arrayIndex, _itemsCount);

        public bool Remove(TSource item)
        {
            if (item == null) return false;

            var index = IndexOf(item);

            if (index == -1) return false;

            RemoveAt(index);

            return true;
        }

        public void Insert(int index, TSource item)
        {
            if (index < 0 || index > _itemsCount) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= _buffer.Length) GrowBuffer();

            if (index < _itemsCount)
                Array.Copy(_buffer, index, _buffer, index + 1, _itemsCount - index);

            _buffer[index] = item;
            _itemsCount++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _buffer.Length) throw new ArgumentOutOfRangeException(nameof(index));

            if (index >= _itemsCount) return;

            _itemsCount--;

            Array.Copy(_buffer, index + 1, _buffer, index, _itemsCount - index);
        }

        public readonly TSource this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _buffer.Length) throw new IndexOutOfRangeException(nameof(index));

                return index >= _itemsCount ? default : _buffer[index];
            }

            set
            {
                if (index < 0 || index >= _itemsCount) throw new IndexOutOfRangeException(nameof(index));

                _buffer[index] = value;
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator<TSource> GetEnumerator() => new Enumerator<TSource>(in _buffer, in _itemsCount);

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => new Enumerator<TSource>(in _buffer, in _itemsCount);

        readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator<TSource>(in _buffer, in _itemsCount);

        public readonly void Dispose() => _arrayPool.Return(_buffer);

        private void GrowBuffer()
        {
            var newLength = _buffer.Length * 2;
            var newBuffer = _arrayPool.Rent(newLength);
            var oldBuffer = _buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, _itemsCount);

            _buffer = newBuffer;
            _arrayPool.Return(oldBuffer);
        }
    }
}