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
            get
            {
                if (index >= Length)
                {
                    throw new Exception("Array overflow");
                }

                return index >= itemsCount ? default : buffer[index];
            }
        }

        public readonly int Length;

        private readonly TSource[] buffer;
        private int itemsCount;

        private ListPool(in int length)
        {
            buffer = ArrayPool<TSource>.Shared.Rent(length);
            Length = length;
            itemsCount = 0;
        }

        public static ListPool<TSource> Rent(in int length)
        {
            return new ListPool<TSource>(in length);
        }

        public void Add(in TSource item)
        {
            if (itemsCount >= Length)
            {
                throw new Exception("Array overflow");
            }

            buffer[itemsCount] = item;
            itemsCount++;
        }

        public void Dispose()
        {
            ArrayPool<TSource>.Shared.Return(buffer);
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