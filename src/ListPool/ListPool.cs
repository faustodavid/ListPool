using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace ListPool
{
    public struct ListPool<TSource> : IDisposable, IValueEnumerable<TSource>
    {
        public readonly TSource this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= buffer.Length)
                {
                    throw new Exception("Array overflow");
                }

                return index >= itemsCount ? default : buffer[index];
            }
        }

        public int Length => buffer.Length;

        private readonly ArrayPool<TSource> arrayPool;
        private TSource[] buffer;
        private int itemsCount;

        public ListPool(int length, ArrayPool<TSource> arrayPool = null)
        {
            if (length < 1)
            {
                throw new Exception("Length should be bigger than 0");
            }

            this.arrayPool = arrayPool ?? ArrayPool<TSource>.Shared;
            buffer = this.arrayPool.Rent(length);
            itemsCount = 0;
        }

        public ListPool(IEnumerable<TSource> source, ArrayPool<TSource> arrayPool = null)
        {
            this.arrayPool = arrayPool ?? ArrayPool<TSource>.Shared;

            if (source is ICollection collection)
            {
                buffer = this.arrayPool.Rent(collection.Count);
                collection.CopyTo(buffer, 0);
                itemsCount = collection.Count;
            }
            else
            {
                buffer = this.arrayPool.Rent(100);
                itemsCount = 0;

                foreach (var item in source)
                {
                    Add(item);
                }
            }
        }

        public void Add(in TSource item)
        {
            if (itemsCount >= buffer.Length)
            {
                GrowBuffer();
            }

            buffer[itemsCount] = item;
            itemsCount++;
        }

        private void GrowBuffer()
        {
            var newLength = buffer.Length * 2;
            var newBuffer = arrayPool.Rent(newLength);
            var oldBuffer = buffer;
            Array.Copy(oldBuffer, 0, newBuffer, 0, itemsCount);
            buffer = newBuffer;
            arrayPool.Return(oldBuffer);
        }

        public void Dispose()
        {
            arrayPool.Return(buffer);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator<TSource> GetEnumerator()
        {
            return new Enumerator<TSource>(in buffer, in itemsCount);
        }

        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator()
        {
            return new Enumerator<TSource>(in buffer, in itemsCount);
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<TSource>(in buffer, in itemsCount);
        }
    }
}