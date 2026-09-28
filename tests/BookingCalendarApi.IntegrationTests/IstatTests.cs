using System.Net;
using System.Net.Http.Json;
using System.ServiceModel;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;

using C59Service;

using Microsoft.EntityFrameworkCore;

using static BookingCalendarApi.IntegrationTests.Infrastructure.IperbookingData;

using ApiEnvironment = BookingCalendarApi.Models.Configurations.Environment;

namespace BookingCalendarApi.IntegrationTests
{
    public class IstatTests : ApiTest
    {
        public IstatTests(ApiFixture fixture) : base(fixture) { }

        private EC59ServiceEndpoint C59 => Fixture.IstatService;

        private async Task<(Structure Structure, HttpClient Client)> SetUpAsync(ApiEnvironment environment = ApiEnvironment.Production)
        {
            Iperbooking.Bookings.Add(Booking(42, Room(1, "20230610", "20230613", guests: 2)));
            Iperbooking.Bookings.Add(Booking(43, Room(2, "20230605", "20230610")));
            Iperbooking.Reservations.Add(Reservation(42,
                Guest("7", 1, "Mario", "Rossi", "19800115", g => { g.BirthCountry = "IT"; g.BirthCounty = "RM"; }),
                Guest("8", 1, "Hans", "Muller", "19800115", g => g.BirthCountry = "DE")));
            Iperbooking.Reservations.Add(Reservation(43, Guest("9", 2, "Luigi", "Verdi", "19700101", g => { g.BirthCountry = "IT"; g.BirthCounty = "RM"; })));
            C59.ultimoC59Async(Arg.Any<ultimoC59>()).Returns(new ultimoC59Response(new movimentiC59
            {
                elencoC59 = new[] { new c59PO { dataMovimentazione = new DateTime(2023, 6, 9), totalePresenti = 12 } },
            }));

            var (structure, client) = await StructureWithUserAsync("hotel", environment);
            await Fixture.SeedRoomAsync(structure.Id, "1-20230610-20230613", "2-20230605-20230610");
            return (structure, client);
        }

        private static object Movements(params object[] movements) => new { date = "2023-06-10", prevTotal = 12, movements };

        [Fact]
        public async Task Movements_of_the_day_after_the_last_upload_are_computed()
        {
            var (structure, client) = await SetUpAsync();
            string germany;
            await using (var context = Fixture.CreateContext())
            {
                var description = (await context.Nations.SingleAsync(n => n.Iso == "DE")).Description;
                germany = description[0] + description.Substring(1).ToLowerInvariant();
            }

            var json = await ReadJsonAsync(await client.GetAsync("/api/v1/istat/movements"));

            json["date"]!.GetValue<string>().Should().Be("2023-06-10");
            json["prevTotal"]!.GetValue<long>().Should().Be(12);
            json["movements"]!.AsArray().Select(m => (m!["targa"]!.GetValue<string>(), m["italia"]!.GetValue<bool>(), m["arrivi"]!.GetValue<long>(), m["partenze"]!.GetValue<long>()))
                .Should().Equal(("RM", true, 1L, 1L), (germany, false, 1L, 0L));
            await C59.Received(1).ultimoC59Async(Arg.Is<ultimoC59>(r =>
                r.username == structure.C59Username && r.password == structure.C59Password && r.struttura == structure.C59Struttura));
        }

        [Fact]
        public async Task Movements_are_sent_to_istat_in_production()
        {
            var (structure, client) = await SetUpAsync();
            inviaC59Full? sent = null;
            C59.inviaC59FullAsync(Arg.Do<inviaC59Full>(r => sent = r)).Returns(new inviaC59FullResponse());

            var response = await client.PostAsJsonAsync("/api/v1/istat/movements",
                Movements(new { targa = "rm", italia = true, arrivi = 2, partenze = 1 }, new { targa = "Germania", italia = false, arrivi = 1, partenze = 0 }));

            response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
            sent.Should().NotBeNull();
            (sent!.username, sent.password, sent.struttura).Should().Be((structure.C59Username, structure.C59Password, structure.C59Struttura));
            sent.c59.dataMovimentazione.Should().Be(new DateTime(2023, 6, 10));
            sent.c59.dataMovimentazioneSpecified.Should().BeTrue();
            sent.c59.esercizioAperto.Should().BeTrue();
            (sent.c59.totaleArrivi, sent.c59.totalePartenze, sent.c59.totalePresenti).Should().Be((3L, 1L, 14L));
            sent.c59.movimenti.Select(m => (m.targa, m.italia, m.arrivi, m.partenze)).Should().Equal(("RM", true, 2L, 1L), ("GERMANIA", false, 1L, 0L));
        }

        [Fact]
        public async Task Movements_are_not_sent_in_development()
        {
            var (_, client) = await SetUpAsync(ApiEnvironment.Development);

            (await client.PostAsJsonAsync("/api/v1/istat/movements", Movements(new { targa = "RM", italia = true, arrivi = 1, partenze = 0 }))).EnsureSuccessStatusCode();

            await C59.DidNotReceiveWithAnyArgs().inviaC59FullAsync(default!);
        }

        [Fact]
        public async Task Invalid_movements_are_bad_requests()
        {
            var (_, client) = await SetUpAsync();

            var response = await client.PostAsJsonAsync("/api/v1/istat/movements", Movements(new { targa = "RM", italia = true, arrivi = 0, partenze = 0 }));

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.INVALID_ISTAT_MOVEMENTS);
            await C59.DidNotReceiveWithAnyArgs().inviaC59FullAsync(default!);
        }

        [Fact]
        public async Task Istat_faults_are_bad_requests()
        {
            var (_, client) = await SetUpAsync();
            C59.ultimoC59Async(Arg.Any<ultimoC59>()).Returns<ultimoC59Response>(_ => throw new FaultException("credenziali non valide"));

            var response = await client.GetAsync("/api/v1/istat/movements");

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.ISTAT_ERROR);
            (await ReadJsonAsync(response))["message"]!.GetValue<string>().Should().Contain("credenziali non valide");
        }

        [Fact]
        public async Task Countries_are_the_decapitalized_nations()
        {
            var (_, client) = await SetUpAsync();

            var countries = await client.GetFromJsonAsync<List<string>>("/api/v1/istat/countries", Json);

            countries.Should().Contain("Italia").And.OnlyHaveUniqueItems().And.BeInAscendingOrder();
            countries.Should().OnlyContain(c => c.Length <= 3 || c.Substring(1) != c.Substring(1).ToUpperInvariant());
        }
    }
}
