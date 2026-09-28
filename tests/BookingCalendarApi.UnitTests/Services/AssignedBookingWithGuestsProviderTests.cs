using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AssignedBookingWithGuestsProviderTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly DataContext _dataContext = new();
        private readonly AssignedBookingWithGuestsProvider _provider;

        public AssignedBookingWithGuestsProviderTests()
        {
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<Booking>
            {
                Booking(10, Room(1, "20230610", "20230612"), Room(2, "20230610", "20230612"), Room(3, "20230610", "20230612")),
                Booking(20, "A", "B", BookingStatus.Cancelled, Room(4, "20230610", "20230612")),
                Booking(30, Room(5, "20230610", "20230612")),
                Booking(40, Room(6, "20230610", "20230612")),
            });
            _repository.Seed(new RoomAssignment { Id = TileId(1, "20230610", "20230612"), RoomId = 101 });
            _repository.Seed(new RoomAssignment { Id = TileId(3, "20230610", "20230612"), RoomId = 103 });
            _repository.Seed(new RoomAssignment { Id = TileId(4, "20230610", "20230612"), RoomId = 104 });
            _repository.Seed(new RoomAssignment { Id = TileId(6, "20230610", "20230612"), RoomId = 106 });
            _repository.Seed(new RoomAssignment { Id = TileId(9, "20230610", "20230612"), RoomId = 109 });
            _iperbooking.GetGuests(Arg.Any<string>()).Returns(Guests(
                Reservation(10, Guest("a", 1, "Anna"), Guest("b", 1, ""), Guest("c", 2, "Carlo"), Guest("d", 3, "")),
                Reservation(40, Guest("e", 6, ""))));
            _provider = new AssignedBookingWithGuestsProvider(_repository, new AssignedBookingComposer(_dataContext), _iperbooking, _dataContext);
        }

        [Fact]
        public async Task Returns_assigned_rooms_with_their_named_guests()
        {
            var bookings = await _provider.Get("2023-06-10", "2023-06-20", exactPeriod: false);

            var booking = bookings.Should().ContainSingle().Subject;
            booking.Booking.BookingNumber.Should().Be(10);
            var room = booking.Rooms.Should().ContainSingle().Subject;
            (room.StayId, room.RoomId, room.Arrival, room.Departure, room.RoomName, room.RateId).Should().Be((1L, (long?)101, "20230610", "20230612", "Doppia", "R1"));
            room.Guests.Select(g => g.GuestId).Should().Equal("a");
        }

        [Fact]
        public async Task Requests_bookings_for_the_period_and_guests_of_assigned_bookings()
        {
            await _provider.Get("2023-06-10", "2023-06-20", exactPeriod: false);

            await _iperbooking.Received(1).GetBookings("2023-06-10", "2023-06-20", false);
            await _iperbooking.Received(1).GetGuests("10,40,");
        }

        [Fact]
        public async Task Defaults_to_one_day_exact_period()
        {
            await _provider.Get("2023-06-10");

            await _iperbooking.Received(1).GetBookings("2023-06-10", "2023-06-11", true);
        }

        [Fact]
        public async Task Loads_only_the_assignments_of_the_fetched_stays()
        {
            await _provider.Get("2023-06-10");

            _dataContext.RoomAssignments.Select(a => a.RoomId).Should().BeEquivalentTo(new[] { 101L, 103L, 106L });
        }
    }
}
