using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Internal
{
    internal class GuestIdConverter : JsonConverter<string>
    {
        public GuestIdConverter() : base() { }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetInt32().ToString(),
                _ => throw new JsonException("Unable to read value as string"),
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (int.TryParse(value, out int parsedInt))
            {
                writer.WriteNumberValue(parsedInt);
            }
            else
            {
                writer.WriteStringValue("");
            }
        }
    }
}
