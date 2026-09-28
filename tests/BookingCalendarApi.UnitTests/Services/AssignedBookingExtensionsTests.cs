using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AssignedBookingExtensionsTests
    {
        private static AssignedRoom<BookingGuest> AssignedRoom(long stayId, string arrival, long? roomId)
            => new(stayId, "Doppia", arrival, "20231231", "R1") { RoomId = roomId };

        [Fact]
        public void ExcludeNotAssigned_drops_unassigned_rooms_and_empty_bookings()
        {
            var bookings = new[]
            {
                new AssignedBooking<BookingGuest>(Booking(1)) { Rooms = { AssignedRoom(1, "20230601", 5), AssignedRoom(2, "20230601", null) } },
                new AssignedBooking<BookingGuest>(Booking(2)) { Rooms = { AssignedRoom(3, "20230601", null) } },
            };

            var result = bookings.ExcludeNotAssigned().ToList();

            result.Should().ContainSingle().Which.Booking.BookingNumber.Should().Be(1);
            result[0].Rooms.Select(r => r.StayId).Should().Equal(1);
        }

        [Fact]
        public void SelectByArrival_keeps_rooms_arriving_on_the_date_and_drops_empty_bookings()
        {
            var bookings = new[]
            {
                new AssignedBooking<BookingGuest>(Booking(1)) { Rooms = { AssignedRoom(1, "20230601", 1), AssignedRoom(2, "20230602", 1), AssignedRoom(3, "20230531", 1) } },
                new AssignedBooking<BookingGuest>(Booking(2)) { Rooms = { AssignedRoom(4, "20230603", 1) } },
            };

            var result = bookings.SelectByArrival(new DateTime(2023, 6, 1)).ToList();

            result.Should().ContainSingle().Which.Booking.BookingNumber.Should().Be(1);
            result[0].Rooms.Select(r => r.StayId).Should().Equal(1);
        }

        [Fact]
        public void Stays_ExcludeNotAssigned_drops_stays_without_room()
        {
            var stays = new[]
            {
                new Stay(1, 1, "20230601", "20230602") { RoomId = 3 },
                new Stay(2, 1, "20230601", "20230602"),
            };

            stays.ExcludeNotAssigned().Select(s => s.StayId).Should().Equal(1);
        }

        [Fact]
        public void UseComposer_returns_what_the_composer_produces()
        {
            var composer = Substitute.For<IComposer<int, string>>();
            var source = new[] { 1, 2 };
            composer.Compose(source).Returns(new[] { "composed" });

            source.UseComposer(composer).Should().Equal("composed");
        }
    }
}
