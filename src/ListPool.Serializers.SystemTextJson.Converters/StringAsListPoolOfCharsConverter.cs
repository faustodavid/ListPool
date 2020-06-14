using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ListPool.Serializers.SystemTextJson.Converters
{
    /// <summary>
    /// Converter to serialize string as ListPool<char> and ListPool<char> as string.
    /// Optimize to work with medium/large string to avoid allocations
    /// Deserialize as ListPool<char> is many time faster than with string.
    /// After finishing with the ListPool<char> you must dispose it to free the memory.
    /// </summary>
    public sealed class StringAsListPoolOfCharsConverter : JsonConverter<ListPool<char>>
    {
        /// <summary>
        /// Deserialize string as ListPool<char>.
        /// You must dispose ListPool<char> after finish it use.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override ListPool<char> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            ReadOnlySpan<byte> writtenBytes = reader.ValueSpan;
            ListPool<char> listPool = new ListPool<char>(Encoding.UTF8.GetCharCount(writtenBytes));

            int charsCount = Encoding.UTF8.GetChars(writtenBytes, listPool.GetRawBuffer());
            listPool.SetOffsetManually(charsCount);

            return listPool;
        }

        /// <summary>
        /// Serialize ListPool<char> as string
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, ListPool<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AsSpan());
        }
    }
}
