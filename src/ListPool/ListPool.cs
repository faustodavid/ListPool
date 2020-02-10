using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ListPool
{
    /// <summary>
    ///     Overhead free implementation of IList using ArrayPool.
    ///     With overhead being the class itself regardless the size of the underlying array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class ListPool<T> : IList<T>, IList, IReadOnlyList<T>, IDisposable
    {
        private const int MinimumCapacity = 32;
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
        ///     Construct ListPool and copy the given source into a pooled buffer.
        /// </summary>
        /// <param name="source"></param>
        public ListPool(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is ICollection<T> collection)
            {
                T[] buffer =
                    ArrayPool<T>.Shared.Rent(collection.Count > MinimumCapacity ? collection.Count : MinimumCapacity);

                collection.CopyTo(buffer, 0);

                _buffer = buffer;
                Count = collection.Count;
            }
            else
            {
                _buffer = ArrayPool<T>.Shared.Rent(MinimumCapacity);
                T[] buffer = _buffer;
                Count = 0;
                int count = 0;
                using IEnumerator<T> enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (count < buffer.Length)
                    {
                        buffer[count] = enumerator.Current;
                        count++;
                    }
                    else
                    {
                        Count = count;
                        AddWithResize(enumerator.Current);
                        count++;
                        buffer = _buffer;
                    }
                }

                Count = count;
            }
        }

        /// <summary>
        ///     Construct ListPool and copy source into new pooled buffer
        /// </summary>
        /// <param name="source"></param>
        public ListPool(T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int capacity = source.Length > MinimumCapacity ? source.Length : MinimumCapacity;
            T[] buffer = ArrayPool<T>.Shared.Rent(capacity);
            source.CopyTo(buffer, 0);

            _buffer = buffer;
            Count = source.Length;
        }

        /// <summary>
        ///     Construct ListPool and copy source into new pooled buffer
        /// </summary>
        /// <param name="source"></param>
        public ListPool(ReadOnlySpan<T> source)
        {
            int capacity = source.Length > MinimumCapacity ? source.Length : MinimumCapacity;
            T[] buffer = ArrayPool<T>.Shared.Rent(capacity);
            source.CopyTo(buffer);

            _buffer = buffer;
            Count = source.Length;
        }

        /// <summary>
        ///     Capacity of the underlying pooled array.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        ///     Returns underlying array to the pool
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

#if NETSTANDARD2_1
        [MaybeNull]
#endif
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
                Count = count + 1;
            }
            else
            {
                AddWithResize(item);
            }
        }

        /// <summary>
        ///     Clears the contents of List.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Count = 0;

        /// <summary>
        ///     Contains returns true if the specified element is in the List.
        ///     It does a linear, O(n) search.  Equality is determined by calling
        ///     EqualityComparer&lt;T&gt;.Default.Equals().
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => IndexOf(item) > -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(_buffer, item, 0, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        ///     Inserts an element into this list at a given index. The size of the list
        ///     is increased by one. If required, the capacity of the list is doubled
        ///     before inserting the new element.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            int count = Count;
            T[] buffer = _buffer;

            if (buffer.Length == count)
            {
                int newCapacity = count * 2;
                GrowBuffer(newCapacity);
                buffer = _buffer;
            }

            if (index < count)
            {
                Array.Copy(buffer, index, buffer, index + 1, count - index);
                buffer[index] = item;
                Count++;
            }
            else if (index == count)
            {
                buffer[index] = item;
                Count++;
            }
            else throw new IndexOutOfRangeException(nameof(index));
        }

        public void RemoveAt(int index)
        {
            int count = Count;
            T[] buffer = _buffer;

            if (index >= count) throw new IndexOutOfRangeException(nameof(index));

            count--;
            Array.Copy(buffer, index + 1, buffer, index, count - index);
            Count = count;
        }

#if NETSTANDARD2_1
        [MaybeNull]
#endif
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
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(_buffer, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_buffer, Count);

        public void AddRange(Span<T> items)
        {
            int count = Count;
            T[] buffer = _buffer;

            bool isCapacityEnough = buffer.Length - items.Length - count > 0;
            if (!isCapacityEnough)
            {
                GrowBuffer(buffer.Length + items.Length);
                buffer = _buffer;
            }

            items.CopyTo(buffer.AsSpan().Slice(count));
            Count += items.Length;
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            int count = Count;
            T[] buffer = _buffer;

            bool isCapacityEnough = buffer.Length - items.Length - count > 0;
            if (!isCapacityEnough)
            {
                GrowBuffer(buffer.Length + items.Length);
                buffer = _buffer;
            }

            items.CopyTo(buffer.AsSpan().Slice(count));
            Count += items.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(T[] array)
        {
            int count = Count;
            T[] buffer = _buffer;

            foreach (T item in array)
            {
                if (count < buffer.Length)
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
                    if (count < buffer.Length)
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

        /// <summary>
        ///     Get span of the items added
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _buffer.AsSpan(0, Count);

        /// <summary>
        ///     Get memory of the items added
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => _buffer.AsMemory(0, Count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            T[] oldBuffer = _buffer;
            T[] newBuffer = arrayPool.Rent(oldBuffer.Length * 2);
            int count = oldBuffer.Length;

            Array.Copy(oldBuffer, 0, newBuffer, 0, count);

            newBuffer[count] = item;
            _buffer = newBuffer;
            Count = count + 1;
            arrayPool.Return(oldBuffer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowBuffer(int capacity)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            T[] newBuffer = arrayPool.Rent(capacity);
            T[] oldBuffer = _buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, oldBuffer.Length);

            _buffer = newBuffer;
            arrayPool.Return(oldBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(_buffer, Count);

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _source;
            private readonly int _itemsCount;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(T[] source, int itemsCount)
            {
                _source = source;
                _itemsCount = itemsCount;
                _index = -1;
            }

#if NETSTANDARD2_1
        [MaybeNull]
#endif
            public readonly ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _source[_index];
            }

#if NETSTANDARD2_1
        [MaybeNull]
#endif
            readonly T IEnumerator<T>.Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _source[_index];
            }

#if NETSTANDARD2_1
        [MaybeNull]
#endif
            readonly object? IEnumerator.Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _source[_index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => unchecked(++_index < _itemsCount);

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
}
