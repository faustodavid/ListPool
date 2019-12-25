using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct ListPool<TSource> : IList<TSource>, IReadOnlyList<TSource>, IDisposable,
                                      IValueEnumerable<TSource>

    {
        public readonly int Capacity => _bufferOwner.Buffer.Length;
        public readonly int Count => _itemsCount;
        public readonly bool IsReadOnly => false;

        private BufferOwner<TSource> _bufferOwner;
        private int _itemsCount;
        private const int MinimumCapacity = 128;

        public ListPool(int length)
        {
            _bufferOwner = new BufferOwner<TSource>(length < MinimumCapacity ? MinimumCapacity : length);
            _itemsCount = 0;
        }

        public ListPool(IEnumerable<TSource> source)
        {
            if (source is ICollection<TSource> collection)
            {
                _bufferOwner = new BufferOwner<TSource>(collection.Count);
                _itemsCount = collection.Count;

                collection.CopyTo(_bufferOwner.Buffer, 0);
            }
            else
            {
                _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);
                _itemsCount = 0;

                using var enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Add(enumerator.Current);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TSource item)
        {
            if (!_bufferOwner.IsValid) _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);
            if (_itemsCount >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();

            _bufferOwner.Buffer[_itemsCount++] = item;
        }

        public void Clear() => _itemsCount = 0;
        public readonly bool Contains(TSource item) => IndexOf(item) > -1;

        public readonly int IndexOf(TSource item) => Array.IndexOf(_bufferOwner.Buffer, item, 0, _itemsCount);

        public readonly void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, _itemsCount);

        public bool Remove(TSource item)
        {
            if (item == null) return false;

            int index = IndexOf(item);

            if (index == -1) return false;

            RemoveAt(index);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, TSource item)
        {
            if (index < 0 || index > _itemsCount) throw new ArgumentOutOfRangeException(nameof(index));
            if (!_bufferOwner.IsValid) _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);
            if (index >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();
            if (index < _itemsCount)
                Array.Copy(_bufferOwner.Buffer, index, _bufferOwner.Buffer, index + 1, _itemsCount - index);

            _bufferOwner.Buffer[index] = item;
            _itemsCount++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _bufferOwner.Buffer.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= _itemsCount) return;

            _itemsCount--;

            Array.Copy(_bufferOwner.Buffer, index + 1, _bufferOwner.Buffer, index, _itemsCount - index);
        }

        [MaybeNull]
        public readonly TSource this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _bufferOwner.Buffer.Length || index >= _itemsCount)
                    throw new IndexOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index < 0 || index >= _bufferOwner.Buffer.Length || index >= _itemsCount)
                    throw new IndexOutOfRangeException(nameof(index));

                _bufferOwner.Buffer[index] = value;
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator<TSource> GetEnumerator() =>
            new Enumerator<TSource>(in _bufferOwner.Buffer, in _itemsCount);

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            _itemsCount = 0;
            _bufferOwner.Dispose();
        }
    }
}
