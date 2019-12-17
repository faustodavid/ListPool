using System.Collections.Generic;

namespace ListPool
{
    interface IValueEnumerable<T> : IEnumerable<T>
    {
        new Enumerator<T> GetEnumerator();
    }
}