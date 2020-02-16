using System;

namespace ListPool.Formatters.Utf8Json.Tests
{
    public sealed class CustomObjectWithListPool : CustomObject, IDisposable
    {
        public ListPool<int> List { get; set; }

        public void Dispose() => List?.Dispose();
    }
}
