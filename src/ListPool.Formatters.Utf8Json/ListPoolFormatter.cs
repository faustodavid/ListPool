using System;
using System.Runtime.CompilerServices;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ListPool.Formatters.Utf8Json
{
    public class ListPoolFormatter<T> : IJsonFormatter<ListPool<T>>
    {
        public void Serialize(ref JsonWriter writer, ListPool<T> values, IJsonFormatterResolver formatterResolver)
        {
            if (values == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            ReadOnlySpan<T> buffer = values.AsSpan();

            foreach (T item in buffer)
            {
                formatter.Serialize(ref writer, item, formatterResolver);
                writer.WriteValueSeparator();
            }

            writer.GetBuffer().AsSpan()[writer.CurrentOffset - 1] = (byte)']';
        }

        public ListPool<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            int count = 0;
            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var list = new ListPool<T>();
            reader.ReadIsBeginArrayWithVerify();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                list.Add(formatter.Deserialize(ref reader, formatterResolver));
            }

            return list;
        }

        public ListPool<T> Deserialize(byte[] bytes)
        {
            JsonReader reader = new JsonReader(bytes);
            int count = 0;
            var formatter = StandardResolver.Default.GetFormatterWithVerify<T>();

            var list = new ListPool<T>();
            reader.ReadIsBeginArrayWithVerify();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                list.Add(formatter.Deserialize(ref reader, StandardResolver.Default));
            }

            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Serialize(ListPool<T> listPool)
        {
            JsonWriter writer = new JsonWriter(MemoryPool.GetBuffer());
            Serialize(ref writer, listPool, StandardResolver.Default);
            return writer.ToUtf8ByteArray();
        }
    }
}
