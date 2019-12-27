using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ListPool
{
    internal struct BufferOwner<TSource> : IDisposable
    {
        private const int MinimumCapacity = 128;
        private ArrayPool<TSource> _arrayPool;
        private TSource[] _buffer;
        private bool _isValid;

        public TSource[] Buffer {
            get
            {
                if (!_isValid)
                {
                    _arrayPool = ArrayPool<TSource>.Shared;
                    _buffer = _arrayPool.Rent(MinimumCapacity);
                    _isValid = true;
                }

                return _buffer;
            }
        }

        public BufferOwner(int size)
        {
            _arrayPool = ArrayPool<TSource>.Shared;
            _buffer = _arrayPool.Rent(size < MinimumCapacity ? MinimumCapacity : size);
            _isValid = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GrowDoubleSize()
        {
            int newLength = Buffer.Length * 2;
            TSource[] newBuffer = _arrayPool.Rent(newLength);
            TSource[] oldBuffer = Buffer;

            Array.Copy(oldBuffer, 0, newBuffer, 0, Buffer.Length);

            _buffer = newBuffer;
            _arrayPool.Return(oldBuffer);
        }

        public void Dispose()
        {
            if (!_isValid) return;

            _isValid = false;
            _arrayPool.Return(Buffer);
        }
    }
}
