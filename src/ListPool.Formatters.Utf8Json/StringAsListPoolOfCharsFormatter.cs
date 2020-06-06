using System;
using System.Buffers;
using System.Text;
using Utf8Json;

namespace ListPool.Formatters.Utf8Json
{
    public class StringAsListPoolOfCharsFormatter : IJsonFormatter<ListPool<char>>
    {
        public void Serialize(ref JsonWriter writer, ListPool<char> value, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }

        public ListPool<char> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            ArraySegment<byte> writtenBytes = reader.ReadStringSegmentRaw();
            byte[] byteBuffer = writtenBytes.Array;

            ListPool<char> listPool = new ListPool<char>(Encoding.UTF8.GetCharCount(byteBuffer, 0, writtenBytes.Count));

            int charsCount = Encoding.UTF8.GetChars(byteBuffer, 0, writtenBytes.Count, listPool.UnsafeGetRawArray(), 0);
            listPool.UnsafeSetCount(charsCount);

            return listPool;
        }
    }
}
