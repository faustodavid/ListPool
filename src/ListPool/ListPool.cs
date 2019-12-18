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
        private readonly TSource[] _buffer;
        private int _itemsCount;
        private int _size;

        public readonly int Count => _itemsCount;
        public readonly bool IsReadOnly { get; }

        public void Add(TSource item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("ListPool is readonly.");
            }

            if (_itemsCount >= _size)
            {
                throw new Exception("Array overflow");
            }

            _buffer[_itemsCount] = item;
            _itemsCount++;
        }

        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("ListPool is readonly.");
            }

            _itemsCount = 0;
            for (var i = 0; i < _itemsCount; i++)
            {
                _buffer[i] = default;
            }
        }

        public readonly bool Contains(TSource item)
        {
            if (item == null)
            {
                for (var i = 0; i < _itemsCount; i++)
                {
                    if (_buffer[i] == null) return true;
                }

                return false;
            }

            for (var i = 0; i < _itemsCount; i++)
            {
                if (_buffer[i] == null) continue;

                if (_buffer[i].Equals(item)) return true;
            }

            return false;
        }

        public readonly void CopyTo(TSource[] array, int arrayIndex)
        {
            Array.Copy(_buffer, 0, array, arrayIndex, _itemsCount);
        }

        public bool Remove(TSource item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("ListPool is readonly.");
            }

            if (item == null)
            {
                return false;
            }

            var index = IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        public readonly int IndexOf(TSource item)
        {
            if (item == null)
            {
                return -1;
            }

            for (var i = 0; i < _itemsCount; i++)
            {
                if(_buffer[i] == null) continue;

                if (_buffer[i].Equals(item)) return i;
            }

            return -1;
        }

        public void Insert(int index, TSource item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("ListPool is readonly.");
            }

            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));

            if (index == _size - 1)
            {
                if (_buffer[index] != null)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _itemsCount++;
                _buffer[index] = item;
                return;
            }

            _itemsCount++;

            if (index >= _itemsCount)
            {
                _buffer[index] = item;
            }

            var save = _buffer[index];

            _buffer[index] = item;

            for (var i = ++index; i < _size; i++)
            {
                var swap = _buffer[i];
                _buffer[i] = save;
                save = swap;
            }
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("ListPool is readonly.");
            }

            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));

            if (index >= _itemsCount)
            {
                return;
            }

            _itemsCount--;

            for (var i = index; i < _itemsCount; i++)
            {
                _buffer[i] = _buffer[i + 1];
            }
        }

        private ListPool(in int size, in bool isReadOnly)
        {
            _buffer = ArrayPool<TSource>.Shared.Rent(size);
            _itemsCount = 0;

            _size = size;
            IsReadOnly = isReadOnly;
        }

        public static ListPool<TSource> Rent(in int length, bool isReadOnly = false)
        {
            return new ListPool<TSource>(in length, isReadOnly);
        }

        public readonly TSource this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                return index >= _itemsCount ? default : _buffer[index];
            }

            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("ListPool is readonly.");
                }

                if (index < 0 || index >= _size)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                _buffer[index] = value;
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator<TSource> GetEnumerator()
        {
            return new Enumerator<TSource>(in _buffer, in _itemsCount);
        }

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator()
        {
            return new Enumerator<TSource>(in _buffer, in _itemsCount);
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<TSource>(in _buffer, in _itemsCount);
        }

        public readonly void Dispose()
        {
            ArrayPool<TSource>.Shared.Return(_buffer);
        }
    }
}