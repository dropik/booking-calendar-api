using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class BookingsServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly BookingsService _service;

        public BookingsServiceTests()
        {
            _service = new BookingsService(_iperbooking, _repository);
        }

        private static Booking MarioRossi()
        {
            var booking = Booking(42, "Mario", "Rossi", BookingStatus.Modified,
                Room(2, "20230612", "20230615", guests: 3, roomName: "Tripla", rateId: "T1"),
                Room(1, "20230610", "20230612", guests: 2, roomName: "Doppia", rateId: "D1"));
            booking.LastModified = "20230501101010";
            booking.Deposit = 150.5m;
            booking.DepositConfirmed = true;
            booking.PaymentMethod = BookingPaymentMethod.BT;
            return booking;
        }

        private static Booking LuigiVerdi() => Booking(43, "Luigi", "Verdi", BookingStatus.New, Room(3, "20230611", "20230613"));

        [Fact]
        public async Task Get_returns_the_booking_with_tiles_guests_color_and_rooms()
        {
            _iperbooking.GetBookings("2023-05-27", "2023-06-26", true).Returns(new List<Booking> { LuigiVerdi(), MarioRossi() });
            var guest = Guest("7", 1, "Anna", "Rossi", "19900220");
            guest.BirthCountry = "IT";
            guest.BirthCounty = "RM";
            guest.BirthCity = "Roma";
            _iperbooking.GetGuests("42").Returns(Guests(Reservation(42, guest, Guest("8", 2, "Luca", "Rossi", "")), Reservation(43, Guest("9", 3))));
            _repository.Seed(new ColorAssignment { BookingId = "42", Color = "#ff0000" });
            _repository.Seed(new ColorAssignment { BookingId = "43", Color = "#00ff00" });
            _repository.Seed(new RoomAssignment { Id = TileId(1, "20230610", "20230612"), RoomId = 101 });

            var booking = await _service.Get("42", "2023-06-11");

            booking.Id.Should().Be("42");
            booking.Name.Should().Be("Mario Rossi");
            booking.LastModified.Should().Be("20230501101010");
            booking.Status.Should().Be(BookingStatus.Modified);
            booking.From.Should().Be("2023-06-10");
            booking.To.Should().Be("2023-06-15");
            booking.Deposit.Should().Be(150.5m);
            booking.DepositConfirmed.Should().BeTrue();
            booking.IsBankTransfer.Should().BeTrue();
            booking.Color.Should().Be("#ff0000");

            booking.Tiles.Select(t => (t.Id, t.From, t.Nights, t.RoomType, t.RateId, t.RoomId)).Should().Equal(
                (TileId(2, "20230612", "20230615"), "2023-06-12", 3u, "Tripla", "T1", (long?)null),
                (TileId(1, "20230610", "20230612"), "2023-06-10", 2u, "Doppia", "D1", (long?)101));

            var anna = booking.Tiles[1].Persons.Should().ContainSingle().Subject;
            (anna.Id, anna.BookingId, anna.Name, anna.Surname, anna.DateOfBirth).Should().Be(("7", "42", "Anna", "Rossi", "1990-02-20"));
            (anna.StateOfBirth, anna.ProvinceOfBirth, anna.PlaceOfBirth).Should().Be(("IT", "RM", "Roma"));
            booking.Tiles[0].Persons.Should().ContainSingle().Which.DateOfBirth.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_without_color_and_non_bank_transfer()
        {
            var booking = MarioRossi();
            booking.PaymentMethod = BookingPaymentMethod.CC;
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), true).Returns(new List<Booking> { booking });
            _iperbooking.GetGuests("42").Returns(Guests(Reservation(42)));

            var response = await _service.Get("42", "2023-06-11");

            response.Color.Should().BeNull();
            response.IsBankTransfer.Should().BeFalse();
        }

        [Fact]
        public async Task Get_of_a_booking_outside_the_fetched_period_throws_not_found()
        {
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), true).Returns(new List<Booking> { LuigiVerdi() });
            _iperbooking.GetGuests("42").Returns(Guests());

            var act = () => _service.Get("42", "2023-06-11");

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("No booking found.").Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }

        [Fact]
        public async Task GetByName_filters_by_range_and_name()
        {
            var outOfRange = Booking(44, "Mario", "Bianchi", BookingStatus.New, Room(4, "20230701", "20230702"));
            _iperbooking.GetBookings("2023-06-10", "2023-06-20", false).Returns(new List<Booking> { MarioRossi(), LuigiVerdi(), outOfRange });
            _repository.Seed(new ColorAssignment { BookingId = "42", Color = "#ff0000" });

            var result = await _service.GetByName("2023-06-10", "2023-06-20", "mario");

            var booking = result.Should().ContainSingle().Subject;
            (booking.Id, booking.Name, booking.LastModified, booking.From, booking.To, booking.Occupations, booking.Status, booking.Color)
                .Should().Be(("42", "Mario Rossi", "20230501101010", "2023-06-10", "2023-06-15", 2u, BookingStatus.Modified, "#ff0000"));
        }

        [Fact]
        public async Task GetByName_without_name_returns_every_booking_in_range()
        {
            _iperbooking.GetBookings("2023-06-10", "2023-06-20", false).Returns(new List<Booking> { MarioRossi(), LuigiVerdi() });

            var result = await _service.GetByName("2023-06-10", "2023-06-20", null);

            result.Select(b => (b.Id, b.Color)).Should().Equal(("42", (string?)null), ("43", (string?)null));
        }

        [Fact]
        public async Task GetByPeriod_returns_bookings_with_colors_assignments_and_guest_counts()
        {
            var farAway = Booking(45, Room(5, "20230801", "20230802"));
            _iperbooking.GetBookings("2023-06-10", "2023-06-11", false).Returns(new List<Booking> { MarioRossi(), farAway });
            _repository.Seed(new ColorAssignment { BookingId = "42", Color = "#ff0000" });
            _repository.Seed(new ColorAssignment { BookingId = "45", Color = "#0000ff" });
            _repository.Seed(new RoomAssignment { Id = TileId(2, "20230612", "20230615"), RoomId = 7 });

            var result = await _service.GetByPeriod("2023-06-10", "2023-06-11");

            var booking = result.Should().ContainSingle().Subject;
            (booking.Id, booking.Name, booking.From, booking.To, booking.Status, booking.Color).Should().Be(("42", "Mario Rossi", "2023-06-10", "2023-06-15", BookingStatus.Modified, "#ff0000"));
            (booking.Deposit, booking.DepositConfirmed, booking.IsBankTransfer, booking.LastModified).Should().Be((150.5m, true, true, "20230501101010"));
            booking.Tiles.Select(t => (t.Id, t.From, t.Nights, t.RoomType, t.RateId, t.Persons, t.RoomId)).Should().Equal(
                (TileId(2, "20230612", "20230615"), "2023-06-12", 3u, "Tripla", "T1", 3u, (long?)7),
                (TileId(1, "20230610", "20230612"), "2023-06-10", 2u, "Doppia", "D1", 2u, (long?)null));
        }

        [Fact]
        public async Task GetByPeriod_extends_the_range_to_colliding_bookings()
        {
            // Luigi arrives on 06-11: out of the 06-10 range, but inside the range of the rooms found (06-11..06-14)
            _iperbooking.GetBookings("2023-06-10", "2023-06-10", false).Returns(new List<Booking> { MarioRossi(), LuigiVerdi() });

            var result = await _service.GetByPeriod("2023-06-10", "2023-06-10");

            result.Select(b => b.Id).Should().Equal("42", "43");
        }
    }
}
