using System.ServiceModel;

using BookingCalendarApi.Models.Clients.C59;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.DTO;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using Microsoft.Extensions.Options;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

using C59Credentials = BookingCalendarApi.Models.C59Service.Credentials;
using Environment = BookingCalendarApi.Models.Configurations.Environment;
using GuestModel = BookingCalendarApi.Models.Iperbooking.Guests.Guest;

namespace BookingCalendarApi.UnitTests.Services
{
    public class IstatServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IStructureService _structureService = Substitute.For<IStructureService>();
        private readonly IAssignedBookingWithGuestsProvider _bookings = Substitute.For<IAssignedBookingWithGuestsProvider>();
        private readonly IC59Client _c59 = Substitute.For<IC59Client>();

        public IstatServiceTests()
        {
            _structureService.GetC59Credentials().Returns(new C59Credentials { Username = "c59-user", Password = "c59-pwd", Struttura = 555 });
            _c59.UltimoC59Async(Arg.Any<UltimoC59Request>()).Returns(LastUpload(new DateTime(2023, 6, 9), 12));
            _bookings.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<AssignedBooking<GuestModel>>());
            _repository.Seed(new Nation("DE", 100000216, "GERMANIA"));
            _repository.Seed(new Nation("GB", 100000219, "REGNO UNITO"));
        }

        private IstatService Service(Environment environment = Environment.Production)
            => new(_structureService, _bookings, _repository, _c59, Options.Create(new ApiSettings { Environment = environment }));

        private static UltimoC59Response LastUpload(DateTime date, long present)
            => new() { Return = new MovimentiC59 { ElencoC59 = new[] { new C59PO { DataMovimentazione = date, TotalePresenti = present } } } };

        private static GuestModel Guest(string country, string? county = null)
        {
            var guest = Fixtures.Guest("1", 1);
            guest.BirthCountry = country;
            guest.BirthCounty = county;
            return guest;
        }

        private static AssignedBooking<GuestModel> Stay(string arrival, string departure, params GuestModel[] guests) => new(Booking(1))
        {
            Rooms = { new AssignedRoom<GuestModel>(1, "Doppia", arrival, departure, "R1") { RoomId = 1, Guests = guests.ToList() } },
        };

        private static MovimentoWSPO Movement(string targa, long arrivals, long departures, bool italy = true)
            => new() { Targa = targa, Arrivi = arrivals, Partenze = departures, Italia = italy };

        [Fact]
        public async Task GetMovements_starts_the_day_after_the_last_upload()
        {
            var movements = await Service().GetMovements();

            movements.Date.Should().Be("2023-06-10");
            movements.PrevTotal.Should().Be(12);
            movements.Movements.Should().BeEmpty();
            await _c59.Received(1).UltimoC59Async(Arg.Is<UltimoC59Request>(r => r.Username == "c59-user" && r.Password == "c59-pwd" && r.Struttura == 555));
            await _bookings.Received(1).Get("2023-06-10", "2023-06-11", false);
        }

        [Fact]
        public async Task GetMovements_counts_arrivals_and_departures_per_province_or_country()
        {
            _bookings.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<AssignedBooking<GuestModel>>
            {
                Stay("20230610", "20230612", Guest("IT", "RM"), Guest("IT", "RM"), Guest("DE")),
                Stay("20230605", "20230610", Guest("IT", "RM"), Guest("GB")),
                Stay("20230601", "20230620", Guest("IT", "MI")),
            });

            var movements = await Service().GetMovements();

            movements.Movements.Select(m => (m.Targa, m.Italia, m.Arrivi, m.Partenze)).Should().Equal(
                ("RM", true, 2L, 1L),
                ("Germania", false, 1L, 0L),
                ("Regno Unito", false, 0L, 1L));
        }

        [Fact]
        public async Task GetMovements_skips_guests_without_origin()
        {
            _bookings.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<AssignedBooking<GuestModel>>
            {
                Stay("20230610", "20230612", Guest(null), Guest(" "), Guest("IT"), Guest("IT", " "), Guest("DE")),
            });

            var movements = await Service().GetMovements();

            movements.Movements.Select(m => m.Targa).Should().Equal("Germania");
        }

        [Fact]
        public async Task GetMovements_uses_an_empty_targa_for_unknown_countries()
        {
            _bookings.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<AssignedBooking<GuestModel>>
            {
                Stay("20230610", "20230612", Guest("XX")),
            });

            (await Service().GetMovements()).Movements.Single().Targa.Should().BeEmpty();
        }

        [Fact]
        public async Task GetMovements_maps_fault_exceptions_to_istat_errors()
        {
            _c59.UltimoC59Async(Arg.Any<UltimoC59Request>()).Returns<UltimoC59Response>(_ => throw new FaultException("invalid credentials"));

            var act = () => Service().GetMovements();

            var exception = (await act.Should().ThrowAsync<BookingCalendarException>()).Which;
            exception.ErrorCode.Should().Be(BCError.ISTAT_ERROR);
            exception.Message.Should().Contain("invalid credentials");
        }

        [Fact]
        public async Task GetMovements_maps_communication_exceptions_to_connection_errors()
        {
            _c59.UltimoC59Async(Arg.Any<UltimoC59Request>()).Returns<UltimoC59Response>(_ => throw new CommunicationException("offline"));

            var act = () => Service().GetMovements();

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("*offline*").Which.ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
        }

        [Fact]
        public async Task SendMovements_sends_totals_and_upper_cased_movements_in_production()
        {
            InviaC59FullRequest? sent = null;
            await _c59.InviaC59FullAsync(Arg.Do<InviaC59FullRequest>(r => sent = r));

            await Service().SendMovements(new IstatMovementsDTO
            {
                Date = "2023-06-10",
                PrevTotal = 12,
                Movements = { Movement("rm", 3, 1), Movement("Germania", 0, 2, italy: false) },
            });

            sent.Should().NotBeNull();
            (sent!.Username, sent.Password, sent.Struttura).Should().Be(("c59-user", "c59-pwd", 555L));
            var c59 = sent.C59;
            c59.DataMovimentazione.Should().Be(new DateTime(2023, 6, 10));
            c59.DataMovimentazioneSpecified.Should().BeTrue();
            c59.EsercizioAperto.Should().BeTrue();
            (c59.TotaleArrivi, c59.TotalePartenze, c59.TotalePresenti).Should().Be((3L, 3L, 12L));
            (c59.UnitaAbitativeDisponibili, c59.UnitaAbitativeOccupate).Should().Be((11L, 0L));
            c59.Movimenti.Select(m => (m.Targa, m.Italia, m.Arrivi, m.Partenze)).Should().Equal(("RM", true, 3L, 1L), ("GERMANIA", false, 0L, 2L));
        }

        [Fact]
        public async Task SendMovements_computes_the_new_total_of_present_guests()
        {
            InviaC59FullRequest? sent = null;
            await _c59.InviaC59FullAsync(Arg.Do<InviaC59FullRequest>(r => sent = r));

            await Service().SendMovements(new IstatMovementsDTO { Date = "2023-06-10", PrevTotal = 10, Movements = { Movement("RM", 5, 2), Movement("MI", 1, 0) } });

            sent!.C59.TotalePresenti.Should().Be(14);
        }

        [Fact]
        public async Task SendMovements_is_not_sent_outside_production()
        {
            await Service(Environment.Development).SendMovements(new IstatMovementsDTO { Date = "2023-06-10", Movements = { Movement("RM", 1, 0) } });

            await _c59.DidNotReceiveWithAnyArgs().InviaC59FullAsync(default!);
        }

        [Theory]
        [InlineData("RM", 1, 0, "RM", 0, 1, "Duplicated entries found.")]
        [InlineData("RM", -1, 2, "MI", 1, 0, "Arrivals can not be negative.")]
        [InlineData("RM", 1, -1, "MI", 1, 0, "Departures can not be negative.")]
        [InlineData("RM", 0, 0, "MI", 1, 0, "Either arrivals or departures must be set.")]
        public async Task SendMovements_rejects_invalid_movements(
            string targa1, long arrivals1, long departures1, string targa2, long arrivals2, long departures2, string message)
        {
            var act = () => Service().SendMovements(new IstatMovementsDTO
            {
                Date = "2023-06-10",
                Movements = { Movement("AG", 1, 1), Movement(targa1, arrivals1, departures1), Movement(targa2, arrivals2, departures2) },
            });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage(message)
                .Which.ErrorCode.Should().Be(BCError.INVALID_ISTAT_MOVEMENTS);
            await _c59.DidNotReceiveWithAnyArgs().InviaC59FullAsync(default!);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public async Task SendMovements_accepts_movements_with_only_arrivals_or_departures(long arrivals, long departures)
        {
            await Service().SendMovements(new IstatMovementsDTO { Date = "2023-06-10", Movements = { Movement("RM", arrivals, departures) } });

            await _c59.ReceivedWithAnyArgs(1).InviaC59FullAsync(default!);
        }

        [Fact]
        public async Task SendMovements_maps_fault_exceptions_to_istat_errors()
        {
            _c59.InviaC59FullAsync(Arg.Any<InviaC59FullRequest>()).Returns(_ => throw new FaultException("rejected"));

            var act = () => Service().SendMovements(new IstatMovementsDTO { Date = "2023-06-10", Movements = { Movement("RM", 1, 0) } });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("*rejected*").Which.ErrorCode.Should().Be(BCError.ISTAT_ERROR);
        }

        [Fact]
        public async Task SendMovements_maps_communication_exceptions_to_connection_errors()
        {
            _c59.InviaC59FullAsync(Arg.Any<InviaC59FullRequest>()).Returns(_ => throw new CommunicationException("offline"));

            var act = () => Service().SendMovements(new IstatMovementsDTO { Date = "2023-06-10", Movements = { Movement("RM", 1, 0) } });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("*offline*").Which.ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
        }

        [Fact]
        public async Task GetCountries_returns_distinct_sorted_decapitalized_nation_names()
        {
            _repository.Seed(new Nation("TT", 1, "TRINIDAD E TOBAGO"));
            _repository.Seed(new Nation("CI", 2, "COSTA D'AVORIO"));
            _repository.Seed(new Nation("US", 3, "U.S.A."));
            _repository.Seed(new Nation("KN", 4, "ST. KITTS"));
            _repository.Seed(new Nation("X1", 5, "USA"));
            _repository.Seed(new Nation("X2", 6, "AB''CD"));
            _repository.Seed(new Nation("X3", 7, "EF..GH"));
            _repository.Seed(new Nation("X4", 8, "GERMANIA"));

            var countries = await Service().GetCountries();

            countries.Should().Equal("Ab''Cd", "Costa D'Avorio", "Ef..Gh", "Germania", "Regno Unito", "St. Kitts", "Trinidad e Tobago", "U.S.A.", "USA");
        }
    }
}
