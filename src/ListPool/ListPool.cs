using System;
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
        private BufferOwner<TSource> _bufferOwner;

        [NonSerialized]
        private object? _syncRoot;

        /// <summary>
        ///     Construct ListPool with default capacity.
        ///     We recommend to indicate the required capacity in front to avoid regrowing as much as possible.
        /// </summary>
        public ListPool()
        {
            _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);
        }

        /// <summary>
        ///     Construct ListPool with the indicated capacity.
        /// </summary>
        /// <param name="capacity">Required initial capacity</param>
        public ListPool(int capacity)
        {
            _bufferOwner = new BufferOwner<TSource>(capacity < MinimumCapacity ? MinimumCapacity : capacity);
        }

        /// <summary>
        ///     Construct ListPool from the given source.
        /// </summary>
        /// <param name="source"></param>
        public ListPool(IEnumerable<TSource> source)
        {
            if (source is ICollection<TSource> collection)
            {
                _bufferOwner = new BufferOwner<TSource>(collection.Count);

                collection.CopyTo(_bufferOwner.Buffer, 0);
                Count = collection.Count;
            }
            else
            {
                _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);

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
        public int Capacity => _bufferOwner.Buffer.Length;

        /// <summary>
        ///     Returns underlying array to the pool
        /// </summary>
        public void Dispose()
        {
            Count = 0;
            _bufferOwner.Dispose();
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
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, Count);
        }

        [MaybeNull]
        object IList.this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

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

        /// <summary>
        ///     Count of items added.
        /// </summary>
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TSource item)
        {
            if (Count >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();

            _bufferOwner.Buffer[Count++] = item;
        }


        public void Clear() => Count = 0;
        public bool Contains(TSource item) => IndexOf(item) > -1;

        public int IndexOf(TSource item) => Array.IndexOf(_bufferOwner.Buffer, item, 0, Count);

        public void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, Count);

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
            if (index >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();
            if (index < Count)
                Array.Copy(_bufferOwner.Buffer, index, _bufferOwner.Buffer, index + 1, Count - index);

            _bufferOwner.Buffer[index] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count) throw new IndexOutOfRangeException(nameof(index));

            Count--;
            Array.Copy(_bufferOwner.Buffer, index + 1, _bufferOwner.Buffer, index, Count - index);
        }

        [MaybeNull]
        public TSource this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                _bufferOwner.Buffer[index] = value;
            }
        }

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueEnumerator<TSource> GetEnumerator() =>
            new ValueEnumerator<TSource>(in _bufferOwner.Buffer, Count);

        public void AddRange(Span<TSource> items)
        {
            bool isCapacityEnough = _bufferOwner.Buffer.Length - items.Length - Count > 0;
            if (!isCapacityEnough)
                _bufferOwner.Grow(_bufferOwner.Buffer.Length + items.Length);

            items.CopyTo(_bufferOwner.Buffer.AsSpan().Slice(Count));
            Count += items.Length;
        }

        public void AddRange(ReadOnlySpan<TSource> items)
        {
            bool isCapacityEnough = _bufferOwner.Buffer.Length - items.Length - Count > 0;
            if (!isCapacityEnough)
                _bufferOwner.Grow(_bufferOwner.Buffer.Length + items.Length);

            items.CopyTo(_bufferOwner.Buffer.AsSpan().Slice(Count));
            Count += items.Length;
        }

        public void AddRange(TSource[] array) => AddRange(array.AsSpan());

        public void AddRange(IEnumerable<TSource> items)
        {
            if (items is ICollection<TSource> collection)
            {
                bool isCapacityEnough = _bufferOwner.Buffer.Length - collection.Count - Count > 0;
                if (!isCapacityEnough)
                    _bufferOwner.Grow(_bufferOwner.Buffer.Length + collection.Count);

                collection.CopyTo(_bufferOwner.Buffer, Count);
                Count += collection.Count;
            }
            else
            {
                foreach (TSource item in items)
                {
                    if (Count >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();
                    _bufferOwner.Buffer[Count++] = item;
                }
            }
        }

        public Span<TSource> AsSpan() => _bufferOwner.Buffer.AsSpan(0, Count);
        public Memory<TSource> AsMemory() => _bufferOwner.Buffer.AsMemory(0, Count);
    }
}
