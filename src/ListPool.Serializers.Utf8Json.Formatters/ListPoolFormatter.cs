using Utf8Json;

namespace ListPool.Serializers.Utf8Json.Formatters
{
    /// <summary>
    /// Formatter for ListPool<T>, Utf8Json by default call the add methods via the interface
    /// with the formatter work directly with the type making it faster.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ListPoolFormatter<T> : IJsonFormatter<ListPool<T>>
    {
        public void Serialize(ref JsonWriter writer, ListPool<T> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();

            IJsonFormatter<T> formatter = formatterResolver.GetFormatterWithVerify<T>();

            if (value.Count > 0)
            {
                formatter.Serialize(ref writer, value[0], formatterResolver);
            }

            foreach (T item in value.AsSpan().Slice(1))
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, item, formatterResolver);
            }

            writer.WriteEndArray();
        }

        public ListPool<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            int count = 0;
            IJsonFormatter<T> formatter = formatterResolver.GetFormatterWithVerify<T>();

            ListPool<T> listPool = new ListPool<T>();
            reader.ReadIsBeginArrayWithVerify();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                listPool.Add(formatter.Deserialize(ref reader, formatterResolver));
            }

            return listPool;
        }
    }
}
