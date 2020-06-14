using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ListPool.Serializers.SystemTextJson.Converters
{
    public sealed class StringAsListPoolOfCharsConverter : JsonConverter<ListPool<char>>
    {
        public override ListPool<char> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            ReadOnlySpan<byte> writtenBytes = reader.ValueSpan;
            ListPool<char> listPool = new ListPool<char>(Encoding.UTF8.GetCharCount(writtenBytes));

            int charsCount = Encoding.UTF8.GetChars(writtenBytes, listPool.GetRawBuffer());
            listPool.SetOffsetManually(charsCount);

            return listPool;
        }

        public override void Write(Utf8JsonWriter writer, ListPool<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AsSpan());
        }
    }
}
