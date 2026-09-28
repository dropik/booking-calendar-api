using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class BookingsExtensionsTests
    {
        [Theory]
        [InlineData("20230605", "20230610", true)]   // departs on the first day
        [InlineData("20230605", "20230609", false)]  // departs the day before
        [InlineData("20230612", "20230615", true)]   // arrives on the last day
        [InlineData("20230613", "20230615", false)]  // arrives the day after
        [InlineData("20230601", "20230620", true)]   // spans the whole range
        public void SelectInRange_keeps_bookings_with_a_room_overlapping_the_range(string arrival, string departure, bool expected)
        {
            var bookings = new[] { Booking(1, Room(1, arrival, departure)) };

            bookings.SelectInRange("2023-06-10", "2023-06-12").Any().Should().Be(expected);
        }

        [Fact]
        public void SelectInRange_needs_only_one_overlapping_room()
        {
            var bookings = new[] { Booking(1, Room(1, "20230101", "20230102"), Room(2, "20230611", "20230612")) };

            bookings.SelectInRange("2023-06-10", "2023-06-12").Should().ContainSingle();
        }

        private static readonly Booking Spanning = Booking(1, Room(1, "20230605", "20230615"));
        private static readonly Booking Inner = Booking(2, Room(2, "20230609", "20230611"));
        private static readonly Booking DepartsOnExtendedStart = Booking(3, Room(3, "20230601", "20230606"));
        private static readonly Booking DepartsBeforeExtendedStart = Booking(4, Room(4, "20230601", "20230605"));
        private static readonly Booking ArrivesOnExtendedEnd = Booking(5, Room(5, "20230614", "20230620"));
        private static readonly Booking ArrivesAfterExtendedEnd = Booking(6, Room(6, "20230615", "20230620"));

        private static readonly Booking[] ExtensionCandidates =
        {
            Spanning, Inner, DepartsOnExtendedStart, DepartsBeforeExtendedStart, ArrivesOnExtendedEnd, ArrivesAfterExtendedEnd,
        };

        [Fact]
        public void SelectInRange_without_auto_extend_keeps_only_the_requested_range()
        {
            ExtensionCandidates.SelectInRange("2023-06-10", "2023-06-10")
                .Should().Equal(Spanning, Inner);
        }

        [Fact]
        public void SelectInRange_with_auto_extend_widens_to_the_rooms_found_minus_their_edge_days()
        {
            // found rooms span 06-05..06-15, so the extended range is 06-06..06-14
            ExtensionCandidates.SelectInRange("2023-06-10", "2023-06-10", autoExtend: true)
                .Should().Equal(Spanning, Inner, DepartsOnExtendedStart, ArrivesOnExtendedEnd);
        }

        [Fact]
        public void SelectInRange_with_auto_extend_and_nothing_in_range_returns_nothing()
        {
            ExtensionCandidates.SelectInRange("2024-01-01", "2024-01-02", autoExtend: true).Should().BeEmpty();
        }

        [Theory]
        [InlineData("rossi", true)]
        [InlineData("MARIO R", true)]
        [InlineData("mario rossi", true)]
        [InlineData("luigi", false)]
        [InlineData("rossi mario", false)]
        public void SelectByName_matches_full_name_case_insensitively(string name, bool expected)
        {
            var bookings = new[] { Booking(1, "Mario", "Rossi", BookingStatus.New) };

            bookings.SelectByName(name).Any().Should().Be(expected);
        }

        [Fact]
        public void SelectById_matches_the_booking_number()
        {
            var bookings = new[] { Booking(12, Room(1, "20230101", "20230102")), Booking(123, Room(2, "20230101", "20230102")) };

            bookings.SelectById("12").Should().ContainSingle().Which.BookingNumber.Should().Be(12);
        }

        [Fact]
        public void ExcludeCancelled_drops_only_cancelled_bookings()
        {
            var bookings = new[]
            {
                Booking(1, "A", "A", BookingStatus.New),
                Booking(2, "B", "B", BookingStatus.Cancelled),
                Booking(3, "C", "C", BookingStatus.Modified),
            };

            bookings.ExcludeCancelled().Select(b => b.BookingNumber).Should().Equal(1, 3);
        }
    }
}
