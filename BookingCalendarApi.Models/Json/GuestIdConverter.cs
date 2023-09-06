using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Json
{
    public class GuestIdConverter : JsonConverter<string>
    {
        public GuestIdConverter() : base() { }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    return reader.GetInt32().ToString();
                default:
                    throw new JsonException("Unable to read value as string");
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
