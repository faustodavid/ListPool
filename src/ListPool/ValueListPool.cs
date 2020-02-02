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
    ///     High-performance implementation of IList with zero heap allocations.
    ///     IMPORTANT:Do not create ValueListPool without indicating a constructor.
    ///     Otherwise it wont work.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public struct ValueListPool<TSource> : IList<TSource>, IList, IReadOnlyList<TSource>, IDisposable,
                                           IValueEnumerable<TSource>

    {
        /// <summary>
        ///     Capacity of the underlying array.
        /// </summary>
        public readonly int Capacity => _buffer.Length;

        /// <summary>
        ///     Count of items added.
        /// </summary>
        public int Count { get; private set; }

        public readonly bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<TSource> AsSpan() => _buffer.AsSpan(0, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Memory<TSource> AsMemory() => _buffer.AsMemory(0, Count);
        int ICollection.Count => Count;
        readonly bool IList.IsFixedSize => false;
        bool ICollection.IsSynchronized => false;
        readonly bool IList.IsReadOnly => false;
        private TSource[] _buffer;
        private const int MinimumCapacity = 128;

        [NonSerialized]
        private object? _syncRoot;

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

        /// <summary>
        ///     Construct ValueListPool with the indicated capacity.
        /// </summary>
        /// <param name="capacity">Required initial capacity</param>
        public ValueListPool(int capacity)
        {
            _syncRoot = null;
            _buffer = ArrayPool<TSource>.Shared.Rent(capacity < MinimumCapacity ? MinimumCapacity : capacity);
            Count = 0;
        }

        /// <summary>
        ///     Construct ValueListPool from the given source.
        /// </summary>
        /// <param name="source"></param>
        public ValueListPool(IEnumerable<TSource> source)
        {
            _syncRoot = null;
            if (source is ICollection<TSource> collection)
            {
                _buffer = ArrayPool<TSource>.Shared.Rent(collection.Count);

                collection.CopyTo(_buffer, 0);
                Count = collection.Count;
            }
            else
            {
                _buffer = ArrayPool<TSource>.Shared.Rent(MinimumCapacity);
                Count = 0;

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
            if (Count >= _buffer.Length) GrowBufferDoubleSize();

            _buffer[Count++] = item;
        }

        int IList.Add(object item)
        {
            if (item is TSource itemAsTSource)
            {
                Add(itemAsTSource);
            }
            else
            {
                throw new ArgumentException($"Wrong type. Expected {typeof(TSource)}, actual: '{item}'.", nameof(item));
            }

            return Count - 1;
        }

        public void AddRange(Span<TSource> items)
        {
            bool isCapacityEnough = _buffer.Length - items.Length - Count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(Count));
            Count += items.Length;
        }

        public void AddRange(ReadOnlySpan<TSource> items)
        {
            bool isCapacityEnough = _buffer.Length - items.Length - Count > 0;
            if (!isCapacityEnough)
                GrowBuffer(_buffer.Length + items.Length);

            items.CopyTo(_buffer.AsSpan().Slice(Count));
            Count += items.Length;
        }

        public void AddRange(TSource[] array) => AddRange(array.AsSpan());

        public void AddRange(IEnumerable<TSource> items)
        {
            if (items is ICollection<TSource> collection)
            {
                bool isCapacityEnough = _buffer.Length - collection.Count - Count > 0;
                if (!isCapacityEnough)
                    GrowBuffer(_buffer.Length + collection.Count);

                collection.CopyTo(_buffer, Count);
                Count += collection.Count;
            }
            else
            {
                foreach (TSource item in items)
                {
                    if (Count >= _buffer.Length) GrowBufferDoubleSize();
                    _buffer[Count++] = item;
                }
            }
        }

        public void Clear() => Count = 0;
        public readonly bool Contains(TSource item) => IndexOf(item) > -1;

        bool IList.Contains(object item)
        {
            if (item is TSource itemAsTSource)
            {
                return Contains(itemAsTSource);
            }

            throw new ArgumentException($"Wrong type. Expected {typeof(TSource)}, actual: '{item}'.", nameof(item));
        }

        int IList.IndexOf(object item)
        {
            if (item is TSource itemAsTSource)
            {
                return IndexOf(itemAsTSource);
            }

            throw new ArgumentException($"Wrong type. Expected {typeof(TSource)}, actual: '{item}'.", nameof(item));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(TSource item) => Array.IndexOf(_buffer, item, 0, Count);

        public readonly void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_buffer, 0, array, arrayIndex, Count);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_buffer, 0, array, arrayIndex, Count);
        }

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
            if (index > Count) throw new IndexOutOfRangeException(nameof(index));
            if (index >= _buffer.Length) GrowBufferDoubleSize();
            if (index < Count)
                Array.Copy(_buffer, index, _buffer, index + 1, Count - index);

            _buffer[index] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count) throw new IndexOutOfRangeException(nameof(index));

            Count--;
            Array.Copy(_buffer, index + 1, _buffer, index, Count - index);
        }

        [MaybeNull]
        readonly object IList.this[int index]
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

        [MaybeNull]
        readonly TSource IList<TSource>.this[int index]
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

        [MaybeNull]
        readonly TSource IReadOnlyList<TSource>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _buffer[index];
            }
        }

        [MaybeNull]
        public readonly ref TSource this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return ref _buffer[index];
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ValueEnumerator<TSource> GetEnumerator() =>
            new ValueEnumerator<TSource>(_buffer, Count);

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        public void Dispose()
        {
            Count = 0;
            if (_buffer != null)
                ArrayPool<TSource>.Shared.Return(_buffer);
        }
    }
}
