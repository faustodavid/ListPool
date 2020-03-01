using System;

namespace ListPool.Resolvers.Utf8Json.Tests
{
    public sealed class CustomObjectWithListPool : CustomObject, IDisposable
    {
        public ListPool<int> List { get; set; }

        public void Dispose() => List?.Dispose();
    }
}
