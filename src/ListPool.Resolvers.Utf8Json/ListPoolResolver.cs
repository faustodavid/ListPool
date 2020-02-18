using System;
using System.Collections.Generic;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ListPool.Resolvers.Utf8Json
{
    public class ListPoolResolver : IJsonFormatterResolver
    {
        private static readonly Dictionary<Type, object> _formatterMap = new Dictionary<Type, object>(14)
        {
            // well known collections
            {typeof(ListPool<Int16>), new ListPoolFormatter<Int16>()},
            {typeof(ListPool<Int32>), new ListPoolFormatter<Int32>()},
            {typeof(ListPool<Int64>), new ListPoolFormatter<Int64>()},
            {typeof(ListPool<UInt16>), new ListPoolFormatter<UInt16>()},
            {typeof(ListPool<UInt32>), new ListPoolFormatter<UInt32>()},
            {typeof(ListPool<UInt64>), new ListPoolFormatter<UInt64>()},
            {typeof(ListPool<Single>), new ListPoolFormatter<Single>()},
            {typeof(ListPool<Double>), new ListPoolFormatter<Double>()},
            {typeof(ListPool<Boolean>), new ListPoolFormatter<Boolean>()},
            {typeof(ListPool<byte>), new ListPoolFormatter<byte>()},
            {typeof(ListPool<SByte>), new ListPoolFormatter<SByte>()},
            {typeof(ListPool<DateTime>), new ListPoolFormatter<DateTime>()},
            {typeof(ListPool<Char>), new ListPoolFormatter<Char>()},
            {typeof(ListPool<string>), new ListPoolFormatter<string>()}
        };
        public IJsonFormatter<T> GetFormatter<T>()
        {
            var ti = typeof(T);

            if (ti.IsGenericType)
            {
                if (_formatterMap.TryGetValue(ti, out object formatter))
                {
                    return (IJsonFormatter<T>)formatter;
                }

                var genericType = ti.GetGenericTypeDefinition();

                if (genericType == typeof(ListPool<>))
                {
                    return (IJsonFormatter<T>)CreateInstance(typeof(ListPoolFormatter<>), ti.GenericTypeArguments);
                }
            }

            return StandardResolver.Default.GetFormatter<T>();

            object CreateInstance(Type genericType, Type[] genericTypeArguments, params object[] arguments)
            {
                return Activator.CreateInstance(genericType.MakeGenericType(genericTypeArguments), arguments);
            }
        }
    }
}
