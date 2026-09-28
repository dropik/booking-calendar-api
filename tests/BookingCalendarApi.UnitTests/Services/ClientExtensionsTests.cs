using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class ClientExtensionsTests
    {
        [Fact]
        public void SelectByStayId_keeps_only_guests_of_the_stay()
        {
            var reservations = new[]
            {
                Reservation(10, Guest("1", 100), Guest("2", 200)),
                Reservation(11, Guest("3", 100)),
            };

            var result = reservations.SelectByStayId("100").ToList();

            result.Select(r => r.ReservationId).Should().Equal(10, 11);
            result.SelectMany(r => r.Guests).Select(g => g.GuestId).Should().Equal("1", "3");
        }

        [Theory]
        [InlineData("ross", new[] { "1" })]
        [InlineData("MARIO ROSSI", new[] { "1" })]
        [InlineData("i", new[] { "1", "2" })]
        [InlineData("zzz", new string[0])]
        public void SelectByQuery_matches_full_name_case_insensitively(string query, string[] expectedIds)
        {
            var reservations = new[] { Reservation(10, Guest("1", 1, "Mario", "Rossi"), Guest("2", 1, "Luigi", "Verdi")) };

            reservations.SelectByQuery(query).SelectMany(r => r.Guests).Select(g => g.GuestId).Should().Equal(expectedIds);
        }

        [Fact]
        public void SelectByQuery_skips_guests_without_first_name()
        {
            var reservations = new[] { Reservation(10, Guest("1", 1, "", "Rossi"), Guest("2", 1, "Mario", "Rossi")) };

            reservations.SelectByQuery("rossi").SelectMany(r => r.Guests).Select(g => g.GuestId).Should().Equal("2");
        }

        [Fact]
        public void ComposeResponse_maps_every_guest()
        {
            var guest = Guest("7", 1, "Mario", "Rossi", "19800131");
            guest.BirthCountry = "IT";
            guest.BirthCounty = "RM";
            guest.BirthCity = "Roma";

            var response = new[] { Reservation(10, guest) }.ComposeResponse().Should().ContainSingle().Subject;

            response.Id.Should().Be("7");
            response.BookingId.Should().Be("10");
            response.Name.Should().Be("Mario");
            response.Surname.Should().Be("Rossi");
            response.DateOfBirth.Should().Be("1980-01-31");
            response.StateOfBirth.Should().Be("IT");
            response.ProvinceOfBirth.Should().Be("RM");
            response.PlaceOfBirth.Should().Be("Roma");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void ComposeResponse_leaves_unknown_birth_date_empty(string birthDate)
        {
            var response = new[] { Reservation(10, Guest("7", 1, birthDate: birthDate)) }.ComposeResponse().Single();

            response.DateOfBirth.Should().BeEmpty();
        }
    }
}
