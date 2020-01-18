using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ListPool
{
    public struct ValueListPool<TSource> : IList<TSource>, IList, IReadOnlyList<TSource>, IDisposable,
                                           IValueEnumerable<TSource>

    {
        public readonly int Capacity =>_bufferOwner.Buffer.Length;
        public int Count { get; private set; }
        public readonly bool IsReadOnly => false;
        public readonly Span<TSource> AsSpan() => _bufferOwner.Buffer.AsSpan(0, Count);
        public readonly Memory<TSource> AsMemory() => _bufferOwner.Buffer.AsMemory(0, Count);
        int ICollection.Count => Count;
        readonly bool IList.IsFixedSize => false;
        bool ICollection.IsSynchronized => false;
        readonly bool IList.IsReadOnly => false;
        private BufferOwner<TSource> _bufferOwner;
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

        public ValueListPool(int length)
        {
            _syncRoot = null;
            _bufferOwner = new BufferOwner<TSource>(length < MinimumCapacity ? MinimumCapacity : length);
            Count = 0;
        }

        public ValueListPool(IEnumerable<TSource> source)
        {
            _syncRoot = null;
            if (source is ICollection<TSource> collection)
            {
                _bufferOwner = new BufferOwner<TSource>(collection.Count);

                collection.CopyTo(_bufferOwner.Buffer, 0);
                Count = collection.Count;
            }
            else
            {
                _bufferOwner = new BufferOwner<TSource>(MinimumCapacity);
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
            if (Count >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();

            _bufferOwner.Buffer[Count++] = item;
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

        public readonly int IndexOf(TSource item) => Array.IndexOf(_bufferOwner.Buffer, item, 0, Count);

        public readonly void CopyTo(TSource[] array, int arrayIndex) =>
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, Count);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_bufferOwner.Buffer, 0, array, arrayIndex, Count);
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
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= _bufferOwner.Buffer.Length) _bufferOwner.GrowDoubleSize();
            if (index < Count)
                Array.Copy(_bufferOwner.Buffer, index, _bufferOwner.Buffer, index + 1, Count - index);

            _bufferOwner.Buffer[index] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

            Count--;
            Array.Copy(_bufferOwner.Buffer, index + 1, _bufferOwner.Buffer, index, Count - index);
        }

        [MaybeNull]
        public readonly TSource this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index < 0 || index >= Count)
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
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _bufferOwner.Buffer[index];
            }

            set
            {
                if (index < 0 || index >= Count)
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
        public readonly ValueEnumerator<TSource> GetEnumerator() =>
            new ValueEnumerator<TSource>(in _bufferOwner.Buffer, Count);

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            Count = 0;
            if (_bufferOwner.IsValid)
                _bufferOwner.Dispose();
        }
    }
}
