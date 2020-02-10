using System;

namespace ListPool.Netstandard2_0.UnitTests.ListPool.Serializer
{
    public sealed class CustomObjectWithListPool : CustomObject, IDisposable
    {
        public ListPool<int> List { get; set; }

        public void Dispose() => List?.Dispose();
    }
}
