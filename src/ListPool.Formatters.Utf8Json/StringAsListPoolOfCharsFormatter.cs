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
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            char[] rawChars = value.UnsafeGetRawBuffer();
            int bytesCount = Encoding.UTF8.GetByteCount(rawChars, 0, value.Count);

            writer.WriteQuotation();

            writer.EnsureCapacity(bytesCount + 2);
            Encoding.UTF8.GetBytes(rawChars, 0, value.Count, writer.GetBuffer().Array, writer.CurrentOffset);
            writer.AdvanceOffset(bytesCount);

            writer.WriteQuotation();
        }

        public ListPool<char> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            ArraySegment<byte> writtenBytes = reader.ReadStringSegmentRaw();
            byte[] byteBuffer = writtenBytes.Array;

            ListPool<char> listPool = new ListPool<char>(Encoding.UTF8.GetCharCount(byteBuffer, 0, writtenBytes.Count));

            int charsCount = Encoding.UTF8.GetChars(byteBuffer, 0, writtenBytes.Count, listPool.UnsafeGetRawBuffer(), 0);
            listPool.UnsafeAdvanceOffset(charsCount);

            return listPool;
        }
    }
}
