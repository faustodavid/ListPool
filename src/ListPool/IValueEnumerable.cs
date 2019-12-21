using System.Collections.Generic;

namespace ListPool
{
    public interface IValueEnumerable<out T, out TEnumerator>
        : IEnumerable<T>
        where TEnumerator 
        : struct
        , IEnumerator<T>
    {
        new TEnumerator GetEnumerator();
    }

    interface IValueEnumerable<T> : IEnumerable<T>
    {
        new Enumerator<T> GetEnumerator();
    }
}