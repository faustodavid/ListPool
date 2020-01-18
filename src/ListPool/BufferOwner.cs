using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ListPool
{
    internal struct BufferOwner<TSource> : IDisposable
    {
        private readonly ArrayPool<TSource> _arrayPool;
        public TSource[] Buffer;
        public bool IsValid;

        public BufferOwner(int size)
        {
            _arrayPool = ArrayPool<TSource>.Shared;
            Buffer = _arrayPool.Rent(size);
            IsValid = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GrowDoubleSize()
        {
            int newLength = Buffer.Length * 2;
            var newBuffer = _arrayPool.Rent(newLength);
            var oldBuffer = Buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, Buffer.Length);

            Buffer = newBuffer;
            _arrayPool.Return(oldBuffer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Grow(int capacity)
        {
            var newBuffer = _arrayPool.Rent(capacity);
            var oldBuffer = Buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, Buffer.Length);

            Buffer = newBuffer;
            _arrayPool.Return(oldBuffer);
        }

        public void Dispose()
        {
            IsValid = false;
            _arrayPool.Return(Buffer);
        }
    }
}
