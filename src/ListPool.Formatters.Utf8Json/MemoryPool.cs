using System;

namespace ListPool.Formatters.Utf8Json
{
    internal static class MemoryPool
    {
        [ThreadStatic]
        private static byte[] _buffer;

        public static byte[] GetBuffer() => _buffer ?? (_buffer = new byte[65536]);
    }
}
