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
    /// <typeparam name="T"></typeparam>
    public sealed class ListPool<T> : IList<T>, IList, IReadOnlyList<T>, IDisposable,
                                      IValueEnumerable<T>

    {
        private const int MinimumCapacity = 128;
        private T[] _buffer;

        [NonSerialized]
        private object? _syncRoot;

        /// <summary>
        ///     Construct ListPool with default capacity.
        ///     We recommend to indicate the required capacity in front to avoid regrowing as much as possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool()
        {
            _buffer = ArrayPool<T>.Shared.Rent(MinimumCapacity);
        }

        /// <summary>
        ///     Construct ListPool with the indicated capacity.
        /// </summary>
        /// <param name="capacity">Required initial capacity</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool(int capacity)
        {
            _buffer = ArrayPool<T>.Shared.Rent(capacity < MinimumCapacity ? MinimumCapacity : capacity);
        }

        /// <summary>
        ///     Construct ListPool from the given source.
        /// </summary>
        /// <param name="source"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListPool(IEnumerable<T> source)
        {
            if (source is ICollection<T> collection)
            {
                T[] buffer = ArrayPool<T>.Shared.Rent(collection.Count);

                collection.CopyTo(buffer, 0);

                _buffer = buffer;
                Count = collection.Count;
            }
            else
            {
                _buffer = ArrayPool<T>.Shared.Rent(MinimumCapacity);

                using IEnumerator<T> enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    // Todo: measure perf of this implementation and if we inline the Add with a stack reference of the buffer
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
            Count = 0;
            T[] buffer = _buffer;
            if (buffer != null)
                ArrayPool<T>.Shared.Return(buffer);
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
            if (item is T itemAsTSource)
            {
                Add(itemAsTSource);
            }
            else
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.",
                    nameof(item));
            }

            return Count - 1;
        }

        bool IList.Contains(object item)
        {
            if (item is T itemAsTSource)
            {
                return Contains(itemAsTSource);
            }

            throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", nameof(item));
        }

        int IList.IndexOf(object item)
        {
            if (item is T itemAsTSource)
            {
                return IndexOf(itemAsTSource);
            }

            throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", nameof(item));
        }

        void IList.Remove(object item)
        {
            if (item is T itemAsTSource)
            {
                Remove(itemAsTSource);
            }
            else if (item != null)
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.",
                    nameof(item));
            }
        }

        void IList.Insert(int index, object item)
        {
            if (item is T itemAsTSource)
            {
                Insert(index, itemAsTSource);
            }
            else
            {
                throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.",
                    nameof(item));
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_buffer, 0, array, arrayIndex, Count);
        }

        [MaybeNull]
        object IList.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _buffer[index];
            }

            set
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                if (value is T valueAsTSource)
                {
                    _buffer[index] = valueAsTSource;
                }
                else
                {
                    throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{value}'.",
                        nameof(value));
                }
            }
        }

        /// <summary>
        ///     Count of items added.
        /// </summary>
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            T[] buffer = _buffer;
            int count = Count;

            if (count < buffer.Length)
            {
                buffer[count] = item;
                Count++;
            }
            else
            {
                AddWithResize(item);
            }
        }

        public void Clear() => Count = 0;
        public bool Contains(T item) => IndexOf(item) > -1;

        public int IndexOf(T item) => Array.IndexOf(_buffer, item, 0, Count);

        public void CopyTo(T[] array, int arrayIndex) =>
            Array.Copy(_buffer, 0, array, arrayIndex, Count);

        public bool Remove(T item)
        {
            if (item is null) return false;

            int index = IndexOf(item);

            if (index == -1) return false;

            RemoveAt(index);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item)
        {
            int count = Count;
            T[] buffer = _buffer;

            if (index < Count)
            {
                if (index < count)
                    Array.Copy(buffer, index, buffer, index + 1, count - index);

                buffer[index] = item;
                Count++;
            }
            else if (index == Count)
            {
                if (index < buffer.Length)
                {
                    buffer[count] = item;
                    Count++;
                }
                else
                {
                    AddWithResize(item);
                }
            }
            else if (index > count) throw new IndexOutOfRangeException(nameof(index));
        }

        public void RemoveAt(int index)
        {
            int count = Count;
            if (index >= count) throw new IndexOutOfRangeException(nameof(index));

            count--;
            Array.Copy(_buffer, index + 1, _buffer, index, count - index);
            Count = count;
        }

        [MaybeNull]
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _buffer[index];
            }

            set
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                _buffer[index] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ValueEnumerator<T>(_buffer, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => new ValueEnumerator<T>(_buffer, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueEnumerator<T> GetEnumerator() => new ValueEnumerator<T>(_buffer, Count);

        public void AddRange(Span<T> items)
        {
            int count = Count;
            bool isCapacityEnough = _buffer.Length - items.Length - count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(count));
            Count += items.Length;
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            int count = Count;
            bool isCapacityEnough = _buffer.Length - items.Length - count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(count));
            Count += items.Length;
        }

        public void AddRange(T[] array) => AddRange(array.AsSpan());

        public void AddRange(IEnumerable<T> items)
        {
            int count = Count;
            T[] buffer = _buffer;

            if (items is ICollection<T> collection)
            {
                bool isCapacityEnough = buffer.Length - collection.Count - count > 0;
                if (!isCapacityEnough)
                {
                    GrowBuffer(buffer.Length + collection.Count);
                    buffer = _buffer;
                }

                collection.CopyTo(buffer, count);
                Count += collection.Count;
            }
            else
            {
                foreach (T item in items)
                {
                    if (count < _buffer.Length)
                    {
                        buffer[count] = item;
                        count++;

                    }
                    else
                    {
                        Count = count;
                        AddWithResize(item);
                        count++;
                        buffer = _buffer;
                    }
                }

                Count = count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _buffer.AsSpan(0, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => _buffer.AsMemory(0, Count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void AddWithResize(T item)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            int newLength = _buffer.Length * 2;
            T[] newBuffer = arrayPool.Rent(newLength);
            T[] oldBuffer = _buffer;
            int count = oldBuffer.Length;

            Array.Copy(oldBuffer, 0, newBuffer, 0, count);

            newBuffer[count] = item;
            _buffer = newBuffer;
            Count++;
            arrayPool.Return(oldBuffer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GrowBuffer(int capacity)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            T[] newBuffer = arrayPool.Rent(capacity);
            T[] oldBuffer = _buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, oldBuffer.Length);

            _buffer = newBuffer;
            arrayPool.Return(oldBuffer);
        }
    }
}
