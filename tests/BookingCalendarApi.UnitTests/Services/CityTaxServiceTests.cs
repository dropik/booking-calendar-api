using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class CityTaxServiceTests
    {
        private const string From = "2023-06-10";
        private const string To = "2023-06-30";

        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly List<Booking> _bookings = new();
        private readonly List<Reservation> _reservations = new();
        private readonly CityTaxService _service;
        private long _nextStay = 1;

        public CityTaxServiceTests()
        {
            var dataContext = new DataContext();
            _iperbooking.GetBookings(From, To, false).Returns(_bookings);
            _iperbooking.GetGuests(Arg.Any<string>()).Returns(_ => new GuestsResponse { Reservations = _reservations });
            _service = new CityTaxService(new AssignedBookingComposer(dataContext), _iperbooking, dataContext, _repository);
        }

        /// <summary>Adds a booking with one assigned stay and the given guests (birth dates).</summary>
        private void Stay(string arrival, string departure, params string[] birthDates) => AddStay(arrival, departure, true, BookingStatus.New, birthDates);

        private void AddStay(string arrival, string departure, bool assigned, BookingStatus status, params string[] birthDates)
        {
            var stayId = _nextStay++;
            _bookings.Add(Booking(stayId * 100, "A", "B", status, Room(stayId, arrival, departure, guests: birthDates.Length)));
            if (assigned)
            {
                _repository.Seed(new RoomAssignment { Id = TileId(stayId, arrival, departure), RoomId = stayId });
            }
            _reservations.Add(Reservation(stayId * 100, birthDates.Select((birthDate, i) => Guest($"{stayId}-{i}", (int)stayId, "Guest", "Name", birthDate)).ToArray()));
        }

        private async Task<(int Standard, int Children, int Over10Days)> CityTax()
        {
            CityTaxResponse response = await _service.Get(From, To);
            return (response.Standard, response.Children, response.Over10Days);
        }

        [Fact]
        public async Task Adult_nights_inside_the_period_are_standard()
        {
            Stay("20230612", "20230615", "19800101");

            (await CityTax()).Should().Be((3, 0, 0));
        }

        [Fact]
        public async Task Nights_of_guests_under_fourteen_at_arrival_are_children()
        {
            Stay("20230612", "20230615", "20090613", "20090612");

            (await CityTax()).Should().Be((3, 3, 0));
        }

        [Fact]
        public async Task Nights_after_the_tenth_are_counted_separately()
        {
            Stay("20230611", "20230623", "19800101");

            (await CityTax()).Should().Be((10, 0, 2));
        }

        [Fact]
        public async Task Exactly_ten_nights_are_all_standard()
        {
            Stay("20230611", "20230621", "19800101");

            (await CityTax()).Should().Be((10, 0, 0));
        }

        [Fact]
        public async Task Nights_before_the_period_are_cropped_from_standard_nights()
        {
            Stay("20230608", "20230613", "19800101");

            (await CityTax()).Should().Be((3, 0, 0));
        }

        [Fact]
        public async Task Cropping_more_than_the_standard_nights_eats_into_the_later_nights()
        {
            // 15 nights = 10 standard + 5 later, 12 of them before the period
            Stay("20230529", "20230613", "19800101");

            (await CityTax()).Should().Be((0, 0, 3));
        }

        [Fact]
        public async Task Nights_after_the_period_are_cropped_from_later_nights_first()
        {
            // 12 nights = 10 standard + 2 later, 3 of them after the period
            Stay("20230621", "20230703", "19800101");

            (await CityTax()).Should().Be((9, 0, 0));
        }

        [Fact]
        public async Task Nights_after_the_period_within_later_nights_keep_standard_nights()
        {
            // 14 nights = 10 standard + 4 later, 3 of them after the period
            Stay("20230619", "20230703", "19800101");

            (await CityTax()).Should().Be((10, 0, 1));
        }

        [Fact]
        public async Task Children_nights_are_cropped_on_both_sides()
        {
            Stay("20230608", "20230702", "20150101");

            (await CityTax()).Should().Be((0, 20, 0));
        }

        [Fact]
        public async Task Children_nights_never_go_negative()
        {
            Stay("20230601", "20230611", "20150101");
            Stay("20230629", "20230710", "20150101");

            (await CityTax()).Should().Be((0, 2, 0));
        }

        [Fact]
        public async Task Standard_nights_cropped_on_both_sides_of_a_long_stay()
        {
            // 40 nights, 5 before and 15 after the period: its 20 nights are the guest's 6th to 25th
            Stay("20230605", "20230715", "19800101");

            (await CityTax()).Should().Be((5, 0, 15));
        }

        [Fact]
        public async Task Guests_of_every_stay_are_summed()
        {
            Stay("20230612", "20230615", "19800101", "20150101");
            Stay("20230620", "20230622", "19800101");

            (await CityTax()).Should().Be((5, 3, 0));
        }

        [Fact]
        public async Task Unassigned_and_cancelled_stays_are_ignored()
        {
            AddStay("20230612", "20230615", false, BookingStatus.New, "19800101");
            AddStay("20230612", "20230615", true, BookingStatus.Cancelled, "19800101");
            Stay("20230620", "20230622", "19800101");

            (await CityTax()).Should().Be((2, 0, 0));
        }

        [Fact]
        public async Task Stays_outside_the_period_are_ignored()
        {
            Stay("20230701", "20230703", "19800101");
            Stay("20230605", "20230609", "19800101");

            (await CityTax()).Should().Be((0, 0, 0));
        }

        [Fact]
        public async Task Guests_without_name_or_birth_date_are_ignored()
        {
            Stay("20230612", "20230615", "19800101", " ");
            _reservations[0].Guests.Add(Guest("x", 1, "", "Name", "19800101"));

            (await CityTax()).Should().Be((3, 0, 0));
        }

        [Fact]
        public async Task Guests_are_requested_for_the_bookings_in_the_period_only()
        {
            Stay("20230612", "20230615", "19800101");
            AddStay("20230612", "20230615", true, BookingStatus.Cancelled, "19800101");
            Stay("20230701", "20230703", "19800101");
            Stay("20230620", "20230622", "19800101");

            await _service.Get(From, To);

            await _iperbooking.Received(1).GetGuests("100,400,");
        }
    }
}
