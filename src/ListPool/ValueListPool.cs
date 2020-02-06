using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ListPool
{
    /// <summary>
    ///     High-performance implementation of IList with zero heap allocations.
    ///     IMPORTANT:Do not create ValueListPool without indicating a constructor.
    ///     Otherwise it wont work.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public ref struct ValueListPool<T> where T : IEquatable<T>
    {
        public enum SourceType
        {
            UseAsInitialBuffer,
            UseAsReferenceData,
            Copy
        }

        private const int MinimumCapacity = 16;
        private T[] _disposableBuffer;
        private Span<T> _buffer;

        /// <summary>
        ///     Construct ListPool with the indicated capacity.
        /// </summary>
        /// <param name="capacity">Required initial capacity</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueListPool(int capacity)
        {
            _disposableBuffer = ArrayPool<T>.Shared.Rent(capacity < MinimumCapacity ? MinimumCapacity : capacity);
            _buffer = _disposableBuffer;
            Count = 0;
        }

        /// <summary>
        ///     Construct the ListPool using the giving source.
        ///     It can use the source as initial buffer in order to reuse the array or
        ///     use the data and wrapper it inside the ListPool or copy the data into new pooled array.
        /// </summary>
        /// <param name="source">source/buffer</param>
        /// <param name="sourceType">Action to take with the source</param>
        public ValueListPool(Span<T> source, SourceType sourceType)
        {
            if (sourceType == SourceType.UseAsInitialBuffer)
            {
                _buffer = source;
                Count = 0;
                _disposableBuffer = null;
            }
            else if (sourceType == SourceType.UseAsReferenceData)
            {
                _buffer = source;
                Count = source.Length;
                _disposableBuffer = null;
            }
            else
            {
                T[] disposableBuffer =
                    ArrayPool<T>.Shared.Rent(source.Length > MinimumCapacity ? source.Length : MinimumCapacity);

                source.CopyTo(disposableBuffer);
                _buffer = disposableBuffer;
                _disposableBuffer = disposableBuffer;
                Count = source.Length;
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
            T[] buffer = _disposableBuffer;
            if (buffer != null)
                ArrayPool<T>.Shared.Return(buffer);
        }

        /// <summary>
        ///     Count of items added.
        /// </summary>
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            Span<T> buffer = _buffer;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Count = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(T item) => IndexOf(item) > -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(T item) => _buffer.Slice(0, Count).IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(Span<T> array) => _buffer.Slice(0, Count).CopyTo(array);

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
            Span<T> buffer = _buffer;

            if (buffer.Length == count)
            {
                int newCapacity = count * 2;
                GrowBuffer(newCapacity);
                buffer = _buffer;
            }

            if (index < count)
            {
                buffer.Slice(index, count).CopyTo(buffer.Slice(index + 1));
                buffer[index] = item;
                Count++;
            }
            else if (index == count)
            {
                buffer[index] = item;
                Count++;
            }
            else throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void RemoveAt(int index)
        {
            int count = Count;
            Span<T> buffer = _buffer;

            if (index >= count) throw new IndexOutOfRangeException(nameof(index));

            count--;
            buffer.Slice(index + 1).CopyTo(buffer.Slice(index));
            Count = count;
        }

        [MaybeNull]
        public readonly ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return ref _buffer[index];
            }
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            int count = Count;
            Span<T> buffer = _buffer;

            bool isCapacityEnough = buffer.Length - items.Length - count > 0;
            if (!isCapacityEnough)
            {
                GrowBuffer(buffer.Length + items.Length);
                buffer = _disposableBuffer;
            }

            items.CopyTo(buffer.Slice(count));
            Count += items.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(T[] array) => AddRange(array.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<T> AsSpan() => _buffer.Slice(0, Count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            if (_disposableBuffer == null)
            {
                Span<T> oldBuffer = _buffer;
                int newSize = oldBuffer.Length * 2;
                T[] newBuffer = arrayPool.Rent(newSize > MinimumCapacity ? newSize : MinimumCapacity);
                oldBuffer.CopyTo(newBuffer);
                newBuffer[oldBuffer.Length] = item;
                _disposableBuffer = newBuffer;
                _buffer = newBuffer;
                Count++;
            }
            else
            {
                T[] oldBuffer = _disposableBuffer;
                T[] newBuffer = arrayPool.Rent(oldBuffer.Length * 2);
                int count = oldBuffer.Length;

                Array.Copy(oldBuffer, 0, newBuffer, 0, count);

                newBuffer[count] = item;
                _disposableBuffer = newBuffer;
                _buffer = newBuffer;
                Count = count + 1;
                arrayPool.Return(oldBuffer);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowBuffer(int capacity)
        {
            ArrayPool<T> arrayPool = ArrayPool<T>.Shared;
            T[] newBuffer = arrayPool.Rent(capacity);
            Span<T> oldBuffer = _buffer;

            oldBuffer.CopyTo(newBuffer);

            _buffer = newBuffer;
            if (_disposableBuffer != null)
            {
                arrayPool.Return(_disposableBuffer);
            }

            _disposableBuffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator() => new Enumerator(_buffer.Slice(0, Count));

        public ref struct Enumerator
        {
            private readonly Span<T> _source;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(Span<T> source)
            {
                _source = source;
                _index = -1;
            }

            [MaybeNull]
            public readonly ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _source[_index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => unchecked(++_index < _source.Length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
