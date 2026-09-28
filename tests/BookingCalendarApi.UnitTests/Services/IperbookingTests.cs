using System.Net;
using System.Text.Json;

using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

namespace BookingCalendarApi.UnitTests.Services
{
    public class IperbookingTests
    {
        private const string BaseUrl = "https://iperbooking.test/v1/";

        private readonly StubHttpMessageHandler _handler = new();
        private readonly Iperbooking _iperbooking;

        public IperbookingTests()
        {
            var structureService = Substitute.For<IStructureService>();
            structureService.GetIperbookingCredentials().Returns(new Auth { IdHotel = "123", Username = "user", Password = "pwd" });
            _iperbooking = new Iperbooking(structureService, new HttpClient(_handler) { BaseAddress = new Uri(BaseUrl) });
        }

        public static IEnumerable<object[]> AllCalls() => new[]
        {
            new object[] { (Func<Iperbooking, Task>)(i => i.GetRoomRates()) },
            new object[] { (Func<Iperbooking, Task>)(i => i.GetBookings("2023-06-01", "2023-06-02")) },
            new object[] { (Func<Iperbooking, Task>)(i => i.GetGuests("1")) },
        };

        [Fact]
        public async Task GetRoomRates_requests_the_rates_of_the_structure()
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json(
                "[{\"roomName\":\"Doppia\",\"rateGroups\":[{\"rates\":[{\"rateId\":\"D1\",\"minOccupancy\":1,\"maxOccupancy\":2,\"baseBoard\":\"BB\"}]}]}]");

            var rates = await _iperbooking.GetRoomRates();

            var request = _handler.Requests.Should().ContainSingle().Subject;
            request.Method.Should().Be(HttpMethod.Get);
            request.Uri.ToString().Should().Be($"{BaseUrl}GetRoomRates.cfm?idhotel=123&username=user&password=pwd&format=json");
            var room = rates.Should().ContainSingle().Subject;
            room.RoomName.Should().Be("Doppia");
            var rate = room.RateGroups.Single().Rates.Single();
            (rate.RateId, rate.MinOccupancy, rate.MaxOccupancy, rate.BaseBoard).Should().Be(("D1", 1u, 2u, "BB"));
        }

        [Fact]
        public async Task GetBookings_widens_the_period_by_thirty_days_on_both_sides()
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json("[]");

            await _iperbooking.GetBookings("2023-06-10", "2023-06-20");

            _handler.Requests.Single().Uri.ToString().Should().Be(
                $"{BaseUrl}GetBookings.cfm?idhotel=123&username=user&password=pwd&format=json&arrivalfrom=20230511&arrivalto=20230720");
        }

        [Fact]
        public async Task GetBookings_with_exact_period_uses_the_given_dates()
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json("[]");

            await _iperbooking.GetBookings("2023-06-10", "2023-06-20", exactPeriod: true);

            _handler.Requests.Single().Uri.Query.Should().EndWith("&arrivalfrom=20230610&arrivalto=20230620");
        }

        [Fact]
        public async Task GetBookings_parses_the_bookings()
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json(
                "[{\"bookingNumber\":42,\"status\":\"Modified\",\"firstName\":\"Mario\",\"lastName\":\"Rossi\",\"lastModified\":\"x\",\"deposit\":10.5," +
                "\"rooms\":[{\"stayId\":7,\"roomName\":\"Doppia\",\"arrival\":\"20230610\",\"departure\":\"20230612\",\"rateId\":\"D1\",\"guests\":[{\"isChild\":true}]}]}]");

            var booking = (await _iperbooking.GetBookings("2023-06-10", "2023-06-20")).Should().ContainSingle().Subject;

            (booking.BookingNumber, booking.Status, booking.FirstName, booking.LastName, booking.Deposit).Should().Be((42L, BookingStatus.Modified, "Mario", "Rossi", 10.5m));
            var room = booking.Rooms.Single();
            (room.StayId, room.Arrival, room.Departure, room.RateId).Should().Be((7L, "20230610", "20230612", "D1"));
            room.Guests.Single().IsChild.Should().BeTrue();
        }

        [Fact]
        public async Task GetGuests_posts_the_credentials_and_reservation_ids()
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json(
                "{\"reservations\":[{\"reservationId\":42,\"guests\":[{\"guestId\":5,\"reservationRoomId\":7,\"firstName\":\"Anna\",\"lastName\":\"Rossi\",\"birthDate\":\"19900101\",\"gender\":\"F\"}]}]}");

            var response = await _iperbooking.GetGuests("42,43,");

            var request = _handler.Requests.Single();
            request.Method.Should().Be(HttpMethod.Post);
            request.Uri.ToString().Should().Be($"{BaseUrl}GetGuests.cfm");
            request.ContentType.Should().StartWith("application/json");
            JsonDocument.Parse(request.Body!).RootElement.GetRawText().Should().Be(
                JsonDocument.Parse("{\"Idhotel\":123,\"Username\":\"user\",\"Password\":\"pwd\",\"ReservationId\":\"42,43,\"}").RootElement.GetRawText());
            var guest = response.Reservations.Single().Guests.Single();
            (guest.GuestId, guest.ReservationRoomId, guest.FirstName, guest.Gender).Should().Be(("5", 7, "Anna", BookingCalendarApi.Models.Iperbooking.Guests.Guest.Sex.F));
        }

        [Theory]
        [MemberData(nameof(AllCalls))]
        public async Task Unsuccessful_responses_are_iperbooking_errors(Func<Iperbooking, Task> call)
        {
            // the body would be valid: only the status tells the call failed
            _handler.Respond = request => StubHttpMessageHandler.Json(
                request.RequestUri!.AbsolutePath.EndsWith("GetGuests.cfm") ? "{}" : "[]", HttpStatusCode.InternalServerError);

            var act = () => call(_iperbooking);

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.IPERBOOKING_ERROR);
        }

        [Theory]
        [MemberData(nameof(AllCalls))]
        public async Task Unreadable_responses_are_iperbooking_errors(Func<Iperbooking, Task> call)
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json("<html>maintenance</html>");

            var act = () => call(_iperbooking);

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.IPERBOOKING_ERROR);
        }

        [Theory]
        [MemberData(nameof(AllCalls))]
        public async Task Null_responses_are_iperbooking_errors(Func<Iperbooking, Task> call)
        {
            _handler.Respond = _ => StubHttpMessageHandler.Json("null");

            var act = () => call(_iperbooking);

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.IPERBOOKING_ERROR);
        }

        [Theory]
        [MemberData(nameof(AllCalls))]
        public async Task Connection_failures_are_connection_errors(Func<Iperbooking, Task> call)
        {
            _handler.Respond = _ => throw new HttpRequestException("no route to host");

            var act = () => call(_iperbooking);

            var exception = (await act.Should().ThrowAsync<BookingCalendarException>()).Which;
            exception.ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
            exception.Message.Should().Contain("no route to host");
        }
    }
}
