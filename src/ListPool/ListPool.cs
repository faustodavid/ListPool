using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ListPool
{
    public struct ListPool<TSource> : IList<TSource>, IList, IReadOnlyList<TSource>, IDisposable,
                                      IValueEnumerable<TSource>

    {
        public readonly int Capacity => _bufferOwner.IsValid ? _bufferOwner.Buffer.Length : 0;
        public readonly int Count => _itemsCount;
        public readonly bool IsReadOnly => false;
        int ICollection.Count => _itemsCount;
        readonly bool IList.IsFixedSize => false;
        bool ICollection.IsSynchronized => false;
        readonly bool IList.IsReadOnly => false;
        private BufferOwner<TSource> _bufferOwner;
        private int _itemsCount;
        private const int MinimumCapacity = 128;

        [NonSerialized]
        private object _syncRoot;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    _ = Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        public ListPool(int length)
        {
            _syncRoot = null;
            _bufferOwner = new BufferOwner<TSource>(length < MinimumCapacity ? MinimumCapacity : length);
            _itemsCount = 0;
        }

        public ListPool(IEnumerable<TSource> source)
        {
            _syncRoot = null;
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

                using IEnumerator<TSource> enumerator = source.GetEnumerator();
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

        int IList.Add(object item)
        {
            if (item is TSource itemAsTSource)
            {
                Add(itemAsTSource);
            }
            else
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{item}'.",
                    nameof(item));
            }

            return Count - 1;
        }


        public void Clear() => _itemsCount = 0;
        public readonly bool Contains(TSource item) => IndexOf(item) > -1;

        bool IList.Contains(object item)
        {
            if (item is TSource itemAsTSource)
            {
                return Contains(itemAsTSource);
            }

            throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{item}'.", nameof(item));
        }

        int IList.IndexOf(object item)
        {
            if (item is TSource itemAsTSource)
            {
                return IndexOf(itemAsTSource);
            }

            throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{item}'.", nameof(item));
        }

        void IList.Remove(object item)
        {
            if (item is null)
            {
                return;
            }

            if (!(item is TSource itemAsTSource))
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{item}'.",
                    nameof(item));
            }

            Remove(itemAsTSource);
        }

        void IList.Insert(int index, object item)
        {
            if (!(item is TSource itemAsTSource))
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{item}'.",
                    nameof(item));
            }

            Insert(index, itemAsTSource);
        }

        public readonly int IndexOf(TSource item) =>
            _bufferOwner.IsValid ? Array.IndexOf(_bufferOwner.Buffer, item, 0, _itemsCount) : -1;

        public readonly void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, _itemsCount);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            // Array.Copy will check for NULL.
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, _itemsCount);
        }

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
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

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
                if (index < 0 || index >= _itemsCount)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index < 0 || index >= _itemsCount)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _bufferOwner.Buffer[index] = value;
            }
        }

        [MaybeNull]
        readonly object IList.this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _itemsCount)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index < 0 || index >= _itemsCount)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (value is TSource valueAsTSource)
                {
                    _bufferOwner.Buffer[index] = valueAsTSource;
                }
                else
                {
                    throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{value}'.",
                        nameof(value));
                }
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
            if (_bufferOwner.IsValid)
                _bufferOwner.Dispose();
        }
    }
}
