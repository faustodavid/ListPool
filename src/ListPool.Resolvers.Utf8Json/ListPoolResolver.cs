using System;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ListPool.Resolvers.Utf8Json
{
    public class ListPoolResolver : IJsonFormatterResolver
    {
        public IJsonFormatter<T> GetFormatter<T>()
        {
            var ti = typeof(T);
            if (ti.IsGenericType)
            {
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
