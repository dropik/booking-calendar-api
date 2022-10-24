using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Internal
{
    public class JsonEnumFromStringConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Was expecting string token");
            }
            try
            {
                var str = reader.GetString();
                return str != null ? Enum.Parse<TEnum>(str, true) : default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
