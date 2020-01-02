using System.Collections.Generic;

namespace ListPool
{
    internal interface IValueEnumerable<T> : IEnumerable<T>
    {
        new ValueEnumerator<T> GetEnumerator();
    }
}
