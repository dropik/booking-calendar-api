using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AssignedBookingComposerTests
    {
        [Fact]
        public void Compose_merges_segments_of_the_same_stay_and_attaches_assignments()
        {
            var dataContext = new DataContext();
            dataContext.RoomAssignments.Add(new RoomAssignment { Id = TileId(1, "20230603", "20230605"), RoomId = 6 });
            dataContext.RoomAssignments.Add(new RoomAssignment { Id = TileId(1, "20230601", "20230603"), RoomId = 5 });
            dataContext.RoomAssignments.Add(new RoomAssignment { Id = TileId(9, "20230601", "20230603"), RoomId = 7 });

            var laterSegment = Room(1, "20230603", "20230605", guests: 2, roomName: "Tripla", rateId: "R3");
            var earlierSegment = Room(1, "20230601", "20230603", guests: 1, roomName: "Doppia", rateId: "R2");
            var otherStay = Room(2, "20230601", "20230602", guests: 1);
            var booking = Booking(42, laterSegment, earlierSegment, otherStay);

            var result = new AssignedBookingComposer(dataContext).Compose(new[] { booking }).Single();

            result.Booking.Should().BeSameAs(booking);
            result.Rooms.Should().HaveCount(2);

            var merged = result.Rooms[0];
            merged.StayId.Should().Be(1);
            merged.Arrival.Should().Be("20230601");
            merged.Departure.Should().Be("20230605");
            merged.RoomName.Should().Be("Tripla");
            merged.RateId.Should().Be("R3");
            merged.RoomId.Should().Be(6);
            merged.Guests.Should().BeSameAs(laterSegment.Guests);

            var unassigned = result.Rooms[1];
            unassigned.StayId.Should().Be(2);
            unassigned.RoomId.Should().BeNull();
        }

        [Fact]
        public void Compose_keeps_every_booking()
        {
            var composer = new AssignedBookingComposer(new DataContext());

            composer.Compose(new[] { Booking(1, Room(1, "20230601", "20230602")), Booking(2) })
                .Select(b => b.Booking.BookingNumber).Should().Equal(1, 2);
        }
    }
}
