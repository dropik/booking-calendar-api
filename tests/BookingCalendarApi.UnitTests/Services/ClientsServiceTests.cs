using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class ClientsServiceTests
    {
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly ClientsService _service;

        public ClientsServiceTests()
        {
            _service = new ClientsService(_iperbooking);
        }

        [Fact]
        public async Task GetByQuery_returns_matching_guests_with_their_booking()
        {
            _iperbooking.GetBookings("2023-06-01", "2023-06-30", true).Returns(new List<Booking>
            {
                Booking(42, "Mario", "Rossi", BookingStatus.New, Room(2, "20230612", "20230615"), Room(1, "20230610", "20230613")),
                Booking(43, "Luigi", "Verdi", BookingStatus.New, Room(3, "20230620", "20230621")),
            });
            var anna = Guest("7", 1, "Anna", "Rossi", "19900220");
            anna.BirthCountry = "IT";
            anna.BirthCounty = "RM";
            anna.BirthCity = "Roma";
            _iperbooking.GetGuests("42,43,").Returns(Guests(
                Reservation(42, anna, Guest("8", 2, "Luca", "Bianchi", "")),
                Reservation(43, Guest("9", 3, "Anna", "Verdi", "19850101")),
                Reservation(44, Guest("10", 4, "Anna", "Neri", "19850101"))));

            var result = await _service.GetByQuery("anna", "2023-06-01", "2023-06-30");

            result.Select(c => (c.Id, c.BookingId, c.Name, c.Surname, c.DateOfBirth, c.BookingName, c.BookingFrom, c.BookingTo)).Should().Equal(
                ("7", "42", "Anna", "Rossi", "1990-02-20", "Mario Rossi", "2023-06-10", "2023-06-15"),
                ("9", "43", "Anna", "Verdi", "1985-01-01", "Luigi Verdi", "2023-06-20", "2023-06-21"));
            (result[0].StateOfBirth, result[0].ProvinceOfBirth, result[0].PlaceOfBirth).Should().Be(("IT", "RM", "Roma"));
        }

        [Fact]
        public async Task GetByQuery_leaves_unknown_birth_date_empty()
        {
            _iperbooking.GetBookings("2023-06-01", "2023-06-30", true).Returns(new List<Booking> { Booking(42, Room(1, "20230610", "20230613")) });
            _iperbooking.GetGuests("42,").Returns(Guests(Reservation(42, Guest("8", 1, "Luca", "Bianchi", ""))));

            var result = await _service.GetByQuery("luca", "2023-06-01", "2023-06-30");

            result.Should().ContainSingle().Which.DateOfBirth.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByTile_returns_the_guests_of_the_stay()
        {
            _iperbooking.GetGuests("42").Returns(Guests(Reservation(42, Guest("7", 1, "Anna"), Guest("8", 2, "Luca"), Guest("9", 1, "Eva"))));

            var result = await _service.GetByTile("42", TileId(1, "20230610", "20230613"));

            result.Select(c => (c.Id, c.BookingId, c.Name)).Should().Equal(("7", "42", "Anna"), ("9", "42", "Eva"));
        }
    }
}
