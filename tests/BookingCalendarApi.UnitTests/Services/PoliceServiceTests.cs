using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using Microsoft.Extensions.Options;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

using Environment = BookingCalendarApi.Models.Configurations.Environment;
using GuestModel = BookingCalendarApi.Models.Iperbooking.Guests.Guest;

namespace BookingCalendarApi.UnitTests.Services
{
    public class PoliceServiceTests
    {
        private readonly IAlloggiatiServiceSession _session = Substitute.For<IAlloggiatiServiceSession>();
        private readonly IAssignedBookingWithGuestsProvider _bookings = Substitute.For<IAssignedBookingWithGuestsProvider>();
        private readonly ITrackedRecordsComposer _composer = Substitute.For<ITrackedRecordsComposer>();
        private readonly InMemoryRepository _repository = new();
        private readonly DataContext _dataContext = new();
        private readonly List<Place> _places = new() { new Place { Codice = 1, Descrizione = "ROMA", Provincia = "RM" } };
        private IEnumerable<AssignedBooking<GuestModel>>? _composed;

        public PoliceServiceTests()
        {
            _repository.Seed(new Nation("IT", 100000100, "ITALIA"));
            _session.GetPlaces().Returns(_places);
            _bookings.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<AssignedBooking<GuestModel>>
            {
                new(Booking(1)) { Rooms = { new AssignedRoom<GuestModel>(1, "Doppia", "20230610", "20230612", "R1") } },
                new(Booking(2)) { Rooms = { new AssignedRoom<GuestModel>(2, "Doppia", "20230609", "20230612", "R1") } },
            });
            _composer.Compose(Arg.Do<IEnumerable<AssignedBooking<GuestModel>>>(source => _composed = source.ToList()))
                .Returns(new[] { "record-1", "record-2" });
        }

        private PoliceService Service(Environment environment = Environment.Production)
            => new(_session, _bookings, _dataContext, _composer, _repository, Options.Create(new ApiSettings { Environment = environment }));

        private static bool Records(IList<string> records) => records.SequenceEqual(new[] { "record-1", "record-2" });

        [Fact]
        public async Task GetRicevuta_returns_the_pdf_of_the_date()
        {
            var pdf = new byte[] { 1, 2 };
            _session.GetRicevuta(new DateTime(2023, 6, 10)).Returns(pdf);

            var result = await Service().GetRicevuta("2023-06-10");

            result.Pdf.Should().BeSameAs(pdf);
            result.FileName.Should().Be("polizia-ricevuta-2023-06-10.pdf");
            Received.InOrder(() =>
            {
                _session.Open();
                _session.GetRicevuta(new DateTime(2023, 6, 10));
            });
        }

        [Fact]
        public async Task Test_sends_the_records_of_the_guests_arriving_on_the_date_in_test_mode()
        {
            await Service().Test(new PoliceSendRequest { Date = "2023-06-10" });

            _composed!.Select(b => b.Booking.BookingNumber).Should().Equal(1);
            Received.InOrder(() =>
            {
                _session.Open();
                _bookings.Get("2023-06-10", null, true);
                _session.SendData(Arg.Is<IList<string>>(r => Records(r)), true);
            });
            await _session.DidNotReceive().SendData(Arg.Any<IList<string>>(), false);
        }

        [Fact]
        public async Task Records_are_composed_with_nations_and_places_loaded()
        {
            await Service().Test(new PoliceSendRequest { Date = "2023-06-10" });

            _dataContext.Nations.Select(n => n.Iso).Should().Equal("IT");
            _dataContext.Places.Should().Equal(_places);
        }

        [Fact]
        public async Task Send_tests_the_records_first_and_then_sends_them_in_production()
        {
            await Service().Send(new PoliceSendRequest { Date = "2023-06-10" });

            Received.InOrder(() =>
            {
                _session.Open();
                _session.SendData(Arg.Is<IList<string>>(r => Records(r)), true);
                _session.SendData(Arg.Is<IList<string>>(r => Records(r)), false);
            });
        }

        [Fact]
        public async Task Send_only_tests_the_records_outside_production()
        {
            await Service(Environment.Development).Send(new PoliceSendRequest { Date = "2023-06-10" });

            await _session.Received(1).SendData(Arg.Any<IList<string>>(), true);
            await _session.DidNotReceive().SendData(Arg.Any<IList<string>>(), false);
        }

        [Fact]
        public async Task GetProvinces_returns_distinct_sorted_provinces_without_foreign_places()
        {
            _places.Clear();
            _places.AddRange(new[] { "RM", "ES", "MI", "RM", "AG" }.Select(p => new Place { Provincia = p }));

            var provinces = await Service().GetProvinces();

            provinces.Should().Equal("AG", "MI", "RM");
            Received.InOrder(() =>
            {
                _session.Open();
                _session.GetPlaces();
            });
        }
    }
}
