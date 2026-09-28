using System.Text.Json;

using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Json;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.UnitTests.Models
{
    public class JsonConvertersTests
    {
        private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

        private static Guest ReadGuest(string guestJsonProperties)
            => JsonSerializer.Deserialize<Guest>($"{{\"reservationRoomId\":1,\"firstName\":\"A\",\"lastName\":\"B\",\"birthDate\":\"\",{guestJsonProperties}}}", Options)!;

        [Fact]
        public void GuestIdConverter_reads_numeric_ids_as_strings()
        {
            ReadGuest("\"guestId\":42").GuestId.Should().Be("42");
        }

        [Fact]
        public void GuestIdConverter_reads_string_ids_as_is()
        {
            ReadGuest("\"guestId\":\"abc\"").GuestId.Should().Be("abc");
        }

        [Fact]
        public void GuestIdConverter_rejects_other_tokens()
        {
            var act = () => ReadGuest("\"guestId\":true");

            act.Should().Throw<JsonException>().WithMessage("Unable to read value as string*");
        }

        [Theory]
        [InlineData("42", "\"GuestId\":42")]
        [InlineData("abc", "\"GuestId\":\"\"")]
        public void GuestIdConverter_writes_numbers_or_empty_string(string guestId, string expectedJson)
        {
            var json = JsonSerializer.Serialize(new Guest(guestId, 1, "A", "B", ""));

            json.Should().Contain(expectedJson);
        }

        [Theory]
        [InlineData("\"f\"", Guest.Sex.F)]
        [InlineData("\"F\"", Guest.Sex.F)]
        [InlineData("\"M\"", Guest.Sex.M)]
        [InlineData("\"unknown\"", Guest.Sex.M)]
        [InlineData("\"\"", Guest.Sex.M)]
        public void JsonEnumFromStringConverter_parses_case_insensitively_and_falls_back_to_default(string value, Guest.Sex expected)
        {
            var options = new JsonSerializerOptions { Converters = { new JsonEnumFromStringConverter<Guest.Sex>() } };

            JsonSerializer.Deserialize<Guest.Sex>(value, options).Should().Be(expected);
        }

        [Fact]
        public void JsonEnumFromStringConverter_parses_document_types()
        {
            ReadGuest("\"guestId\":1,\"docType\":\"pp\"").DocType.Should().Be(Guest.DocumentType.PP);
        }

        [Fact]
        public void JsonEnumFromStringConverter_rejects_non_string_tokens()
        {
            var options = new JsonSerializerOptions { Converters = { new JsonEnumFromStringConverter<Guest.Sex>() } };

            var act = () => JsonSerializer.Deserialize<Guest.Sex>("1", options);

            act.Should().Throw<JsonException>().WithMessage("Was expecting string token*");
        }

        [Fact]
        public void JsonEnumFromStringConverter_writes_enum_name()
        {
            var options = new JsonSerializerOptions { Converters = { new JsonEnumFromStringConverter<Guest.DocumentType>() } };

            JsonSerializer.Serialize(Guest.DocumentType.DL, options).Should().Be("\"DL\"");
        }

        [Fact]
        public void LowerCaseEnumConverter_writes_camel_case_names()
        {
            var response = new ShortBookingResponse("1", "n", "m", "f", "t", 1) { Status = BookingStatus.Cancelled };

            JsonSerializer.Serialize(response).Should().Contain("\"Status\":\"cancelled\"");
        }

        [Fact]
        public void LowerCaseEnumConverter_rejects_integer_values()
        {
            var act = () => JsonSerializer.Deserialize<ShortBookingResponse>("{\"Id\":\"1\",\"Name\":\"n\",\"LastModified\":\"m\",\"From\":\"f\",\"To\":\"t\",\"Occupations\":1,\"Status\":2}");

            act.Should().Throw<JsonException>();
        }

        [Fact]
        public void Booking_status_and_payment_method_are_read_from_strings()
        {
            var booking = JsonSerializer.Deserialize<Booking>(
                "{\"bookingNumber\":7,\"firstName\":\"A\",\"lastName\":\"B\",\"lastModified\":\"x\",\"status\":\"Cancelled\",\"paymentMethod\":\"BT\"}",
                Options)!;

            booking.Status.Should().Be(BookingStatus.Cancelled);
            booking.PaymentMethod.Should().Be(BookingPaymentMethod.BT);
        }
    }
}
