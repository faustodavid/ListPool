using System;
using System.Text;
using Utf8Json;

namespace ListPool.Formatters.Utf8Json
{
    /// <summary>
    /// Formatter to serialize string as ListPool<char> and ListPool<char> as string.
    /// Optimize to work with medium/large string to avoid allocations
    /// Serialize and deserialize as ListPool<char> is many time faster than with string.
    /// After finishing with the ListPool<char> you must dispose it to free the memory.
    /// </summary>
    public sealed class StringAsListPoolOfCharsFormatter : IJsonFormatter<ListPool<char>>
    {
        /// <summary>
        /// Serialize ListPool<char> as string
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        public void Serialize(ref JsonWriter writer, ListPool<char> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            int valueCount = value.Count;
            char[] rawChars = value.GetRawBuffer();
            int bytesCount = Encoding.UTF8.GetByteCount(rawChars, 0, valueCount);

            writer.WriteQuotation();

            writer.EnsureCapacity(bytesCount + 2);
            Encoding.UTF8.GetBytes(rawChars, 0, valueCount, writer.GetBuffer().Array, writer.CurrentOffset);
            writer.AdvanceOffset(bytesCount);

            writer.WriteQuotation();
        }

        /// <summary>
        /// Deserialize string as ListPool<char>.
        /// You must dispose ListPool<char> after finish it use.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        public ListPool<char> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            ArraySegment<byte> writtenBytes = reader.ReadStringSegmentRaw();
            int writtenBytesOffset = writtenBytes.Offset;
            int writtenBytesCount = writtenBytes.Count;
            byte[] byteBuffer = writtenBytes.Array;

            ListPool<char> listPool = new ListPool<char>(Encoding.UTF8.GetCharCount(byteBuffer, writtenBytesOffset, writtenBytesCount));

            int charsCount = Encoding.UTF8.GetChars(byteBuffer, writtenBytesOffset, writtenBytesCount, listPool.GetRawBuffer(), 0);
            listPool.SetOffsetManually(charsCount);

            return listPool;
        }
    }
}
