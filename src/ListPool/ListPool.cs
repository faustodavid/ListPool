using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ListPool
{
    /// <summary>
    ///     Overhead free implementation of IList using ArrayPool.
    ///     With overhead being the class itself regardless the size of the underlying array.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public sealed class ListPool<TSource> : IList<TSource>, IList, IReadOnlyList<TSource>, IDisposable,
                                            IValueEnumerable<TSource>

    {
        private const int MinimumCapacity = 128;
        private TSource[] _buffer;

        [NonSerialized]
        private object? _syncRoot;

        /// <summary>
        ///     Construct ListPool with default capacity.
        ///     We recommend to indicate the required capacity in front to avoid regrowing as much as possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool()
        {
            _buffer = ArrayPool<TSource>.Shared.Rent(MinimumCapacity);
        }

        /// <summary>
        ///     Construct ListPool with the indicated capacity.
        /// </summary>
        /// <param name="capacity">Required initial capacity</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool(int capacity)
        {
            _buffer = ArrayPool<TSource>.Shared.Rent(capacity < MinimumCapacity ? MinimumCapacity : capacity);
        }

        /// <summary>
        ///     Construct ListPool from the given source.
        /// </summary>
        /// <param name="source"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool(IEnumerable<TSource> source)
        {
            if (source is ICollection<TSource> collection)
            {
                _buffer = ArrayPool<TSource>.Shared.Rent(collection.Count);

                collection.CopyTo(_buffer, 0);
                _count = collection.Count;
            }
            else
            {
                _buffer = ArrayPool<TSource>.Shared.Rent(MinimumCapacity);

                using IEnumerator<TSource> enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Add(enumerator.Current);
                }
            }
        }

        /// <summary>
        ///     Capacity of the underlying array.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        ///     Returns underlying array to the pool
        /// </summary>
        public void Dispose()
        {
            _count = 0;
            if (_buffer != null)
                ArrayPool<TSource>.Shared.Return(_buffer);
        }

        int ICollection.Count => Count;
        bool IList.IsFixedSize => false;
        bool ICollection.IsSynchronized => false;
        bool IList.IsReadOnly => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot is null)
                {
                    _ = Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
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

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_buffer, 0, array, arrayIndex, Count);
        }

        [MaybeNull]
        object IList.this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _buffer[index];
            }

            set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException(nameof(index));

                if (value is TSource valueAsTSource)
                {
                    _buffer[index] = valueAsTSource;
                }
                else
                {
                    throw new ArgumentException($"Wrong value type. Expected {typeof(TSource)}, got: '{value}'.",
                        nameof(value));
                }
            }
        }

        /// <summary>
        ///     Count of items added.
        /// </summary>
        public int Count => _count;

        private int _count;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TSource item)
        {
            if (_count >= _buffer.Length) GrowBufferDoubleSize();

            _buffer[_count++] = item;
        }

        public void Clear() => _count = 0;
        public bool Contains(TSource item) => IndexOf(item) > -1;

        public int IndexOf(TSource item) => Array.IndexOf(_buffer, item, 0, _count);

        public void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_buffer, 0, array, arrayIndex, _count);

        public bool Remove(TSource item)
        {
            if (item is null) return false;

            int index = IndexOf(item);

            if (index == -1) return false;

            RemoveAt(index);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, TSource item)
        {
            if (index > _count) throw new IndexOutOfRangeException(nameof(index));
            if (index >= _buffer.Length) GrowBufferDoubleSize();
            if (index < _count)
                Array.Copy(_buffer, index, _buffer, index + 1, _count - index);

            _buffer[index] = item;
            _count++;
        }

        public void RemoveAt(int index)
        {
            if (index >= _count) throw new IndexOutOfRangeException(nameof(index));

            _count--;
            Array.Copy(_buffer, index + 1, _buffer, index, _count - index);
        }

        [MaybeNull]
        public TSource this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _buffer[index];
            }

            set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException(nameof(index));

                _buffer[index] = value;
            }
        }

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueEnumerator<TSource> GetEnumerator() =>
            new ValueEnumerator<TSource>(_buffer, _count);

        public void AddRange(Span<TSource> items)
        {
            bool isCapacityEnough = _buffer.Length - items.Length - _count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(_count));
            _count += items.Length;
        }

        public void AddRange(ReadOnlySpan<TSource> items)
        {
            bool isCapacityEnough = _buffer.Length - items.Length - _count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(_count));
            _count += items.Length;
        }

        public void AddRange(TSource[] array) => AddRange(array.AsSpan());

        public void AddRange(IEnumerable<TSource> items)
        {
            if (items is ICollection<TSource> collection)
            {
                bool isCapacityEnough = _buffer.Length - collection.Count - _count > 0;
                if (!isCapacityEnough)
                    GrowBuffer(_buffer.Length + collection.Count);

                collection.CopyTo(_buffer, _count);
                _count += collection.Count;
            }
            else
            {
                foreach (TSource item in items)
                {
                    if (_count >= _buffer.Length) GrowBufferDoubleSize();
                    _buffer[_count++] = item;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<TSource> AsSpan() => _buffer.AsSpan(0, _count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<TSource> AsMemory() => _buffer.AsMemory(0, _count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GrowBufferDoubleSize()
        {
            int newLength = _buffer.Length * 2;
            var newBuffer = ArrayPool<TSource>.Shared.Rent(newLength);
            var oldBuffer = _buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, _buffer.Length);

            _buffer = newBuffer;
            ArrayPool<TSource>.Shared.Return(oldBuffer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GrowBuffer(int capacity)
        {
            var newBuffer = ArrayPool<TSource>.Shared.Rent(capacity);
            var oldBuffer = _buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, _buffer.Length);

            _buffer = newBuffer;
            ArrayPool<TSource>.Shared.Return(oldBuffer);
        }
    }
}
