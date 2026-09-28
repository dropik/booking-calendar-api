using System.Net;

using BookingCalendarApi.Models.Exceptions;

namespace BookingCalendarApi.UnitTests.Models
{
    public class BookingCalendarExceptionTests
    {
        [Theory]
        [InlineData(BCError.NOT_FOUND, HttpStatusCode.NotFound)]
        [InlineData(BCError.CONNECTION_ERROR, HttpStatusCode.RequestTimeout)]
        [InlineData(BCError.SERVER_ERROR, HttpStatusCode.BadRequest)]
        [InlineData(BCError.ID_CHANGE_ATTEMPT, HttpStatusCode.BadRequest)]
        [InlineData(BCError.ROOMS_COLLISION, HttpStatusCode.BadRequest)]
        public void GetStatusCode_maps_error_codes(int errorCode, HttpStatusCode expected)
        {
            new BookingCalendarException(errorCode, "message").GetStatusCode().Should().Be(expected);
        }

        [Fact]
        public void Message_only_constructor_defaults_to_server_error()
        {
            var exception = new BookingCalendarException("boom");

            exception.ErrorCode.Should().Be(BCError.SERVER_ERROR);
            exception.Message.Should().Be("boom");
        }

        [Fact]
        public void Error_code_only_constructor_keeps_code_with_empty_message()
        {
            var exception = new BookingCalendarException(BCError.NOT_FOUND);

            exception.ErrorCode.Should().Be(BCError.NOT_FOUND);
            exception.Message.Should().BeEmpty();
        }
    }
}
