using System.Net;
using System.Net.Http.Json;
using System.ServiceModel;

using AlloggiatiService;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;

using static BookingCalendarApi.IntegrationTests.Infrastructure.IperbookingData;

using ApiEnvironment = BookingCalendarApi.Models.Configurations.Environment;

namespace BookingCalendarApi.IntegrationTests
{
    public class PoliceTests : ApiTest
    {
        public PoliceTests(ApiFixture fixture) : base(fixture) { }

        private const string Rome = "412058091";

        private static readonly string ExpectedRecord =
            "16" + "10/06/2023" + "03" + "ROSSI".PadRight(50) + "MARIO".PadRight(30) + "1" + "15/01/1980"
            + Rome + "RM" + "100000100" + "100000100" + "IDENT" + "AB123".PadRight(20) + Rome;

        private IServiceSoap Soap => Fixture.PoliceService;

        private static EsitoOperazioneServizio Ok() => new() { esito = true };

        private async Task<(Structure Structure, HttpClient Client)> SetUpAsync(ApiEnvironment environment = ApiEnvironment.Production)
        {
            Iperbooking.Bookings.Add(Booking(42, Room(1, "20230610", "20230613")));
            Iperbooking.Bookings.Add(Booking(43, Room(2, "20230609", "20230613")));
            Iperbooking.Reservations.Add(Reservation(42, Guest("7", 1, "Mario", "Rossi", "19800115", g =>
            {
                g.BirthCountry = "IT";
                g.BirthCounty = "RM";
                g.BirthCity = "Roma";
                g.Citizenship = "IT";
                g.DocType = Models.Iperbooking.Guests.Guest.DocumentType.ID;
                g.DocNumber = "AB123";
                g.DocCity = "Roma";
            })));
            Iperbooking.Reservations.Add(Reservation(43, Guest("8", 2, "Luigi", "Verdi", "19700101", g => g.BirthCountry = "DE")));

            Soap.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns(new GenerateTokenResponse(
                new GenerateTokenResponseBody(new TokenInfo { token = "the-token" }, Ok())));
            Soap.TabellaAsync(Arg.Any<TabellaRequest>()).Returns(new TabellaResponse(new TabellaResponseBody(Ok(),
                $"Codice;Descrizione;Provincia;DataFineVal\n{Rome};ROMA;RM;\n403015146;MILANO;MI;\n9;MADRID;ES;\n")));
            Soap.TestAsync(Arg.Any<TestRequest>()).Returns(new TestResponse(new TestResponseBody(Ok(), new ElencoSchedineEsito { Dettaglio = new[] { Ok() } })));
            Soap.SendAsync(Arg.Any<SendRequest>()).Returns(new SendResponse(new SendResponseBody(Ok(), new ElencoSchedineEsito { Dettaglio = new[] { Ok() } })));

            var (structure, client) = await StructureWithUserAsync("hotel", environment);
            await Fixture.SeedRoomAsync(structure.Id, "1-20230610-20230613", "2-20230609-20230613");
            return (structure, client);
        }

        [Fact]
        public async Task Test_submits_the_records_of_the_guests_arriving_on_the_date()
        {
            var (structure, client) = await SetUpAsync();

            var response = await client.PostAsJsonAsync("/api/v1/police/test", new { date = "2023-06-10" });

            response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
            await Soap.Received(1).GenerateTokenAsync(Arg.Is<GenerateTokenRequest>(r =>
                r.Body.Utente == structure.ASUtente && r.Body.Password == structure.ASPassword && r.Body.WsKey == structure.ASWsKey));
            await Soap.Received(1).TabellaAsync(Arg.Is<TabellaRequest>(r => r.Body.token == "the-token" && r.Body.tipo == TipoTabella.Luoghi));
            await Soap.Received(1).TestAsync(Arg.Is<TestRequest>(r =>
                r.Body.Utente == structure.ASUtente && r.Body.token == "the-token" && r.Body.ElencoSchedine.SequenceEqual(new[] { ExpectedRecord })));
            await Soap.DidNotReceiveWithAnyArgs().SendAsync(default!);
        }

        [Fact]
        public async Task Send_tests_and_then_sends_the_records_in_production()
        {
            var (_, client) = await SetUpAsync();

            (await client.PostAsJsonAsync("/api/v1/police", new { date = "2023-06-10" })).EnsureSuccessStatusCode();

            Received.InOrder(() =>
            {
                Soap.TestAsync(Arg.Is<TestRequest>(r => r.Body.ElencoSchedine.SequenceEqual(new[] { ExpectedRecord })));
                Soap.SendAsync(Arg.Is<SendRequest>(r => r.Body.token == "the-token" && r.Body.ElencoSchedine.SequenceEqual(new[] { ExpectedRecord })));
            });
        }

        [Fact]
        public async Task Send_only_tests_the_records_in_development()
        {
            var (_, client) = await SetUpAsync(ApiEnvironment.Development);

            (await client.PostAsJsonAsync("/api/v1/police", new { date = "2023-06-10" })).EnsureSuccessStatusCode();

            await Soap.ReceivedWithAnyArgs(1).TestAsync(default!);
            await Soap.DidNotReceiveWithAnyArgs().SendAsync(default!);
        }

        [Fact]
        public async Task Rejected_records_are_bad_requests()
        {
            var (_, client) = await SetUpAsync();
            Soap.TestAsync(Arg.Any<TestRequest>()).Returns(new TestResponse(new TestResponseBody(Ok(),
                new ElencoSchedineEsito { Dettaglio = new[] { new EsitoOperazioneServizio { esito = false, ErroreDettaglio = "Data di nascita errata" } } })));

            var response = await client.PostAsJsonAsync("/api/v1/police", new { date = "2023-06-10" });

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.POLICE_SERVICE_ERROR);
            (await ReadJsonAsync(response))["message"]!.GetValue<string>().Should().Be("Data di nascita errata");
            await Soap.DidNotReceiveWithAnyArgs().SendAsync(default!);
        }

        [Fact]
        public async Task Refused_credentials_are_bad_requests()
        {
            var (_, client) = await SetUpAsync();
            Soap.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns(new GenerateTokenResponse(
                new GenerateTokenResponseBody(new TokenInfo(), new EsitoOperazioneServizio { esito = false, ErroreDettaglio = "Utente non valido" })));

            await ShouldBeErrorAsync(await client.PostAsJsonAsync("/api/v1/police/test", new { date = "2023-06-10" }), HttpStatusCode.BadRequest, BCError.POLICE_SERVICE_ERROR);
        }

        [Fact]
        public async Task Unreachable_police_service_is_a_request_timeout()
        {
            var (_, client) = await SetUpAsync();
            Soap.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns<GenerateTokenResponse>(_ => throw new EndpointNotFoundException("no endpoint"));

            await ShouldBeErrorAsync(await client.PostAsJsonAsync("/api/v1/police/test", new { date = "2023-06-10" }), HttpStatusCode.RequestTimeout, BCError.CONNECTION_ERROR);
        }

        [Fact]
        public async Task Ricevuta_is_downloaded_as_pdf()
        {
            var (_, client) = await SetUpAsync();
            var pdf = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            Soap.RicevutaAsync(Arg.Any<RicevutaRequest>()).Returns(new RicevutaResponse(new RicevutaResponseBody(Ok(), pdf)));

            var response = await client.GetAsync("/api/v1/police/ricevuta?date=2023-06-10");

            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
            response.Content.Headers.ContentDisposition!.FileName.Should().Be("polizia-ricevuta-2023-06-10.pdf");
            (await response.Content.ReadAsByteArrayAsync()).Should().Equal(pdf);
            await Soap.Received(1).RicevutaAsync(Arg.Is<RicevutaRequest>(r => r.Body.Data == new DateTime(2023, 6, 10) && r.Body.token == "the-token"));
        }

        [Fact]
        public async Task Provinces_come_from_the_places_table()
        {
            var (_, client) = await SetUpAsync();

            (await client.GetFromJsonAsync<List<string>>("/api/v1/police/provinces", Json)).Should().Equal("MI", "RM");
        }
    }
}
