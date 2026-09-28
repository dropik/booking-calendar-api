using System.ServiceModel;

using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Clients.Police;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Services;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AlloggiatiServiceSessionTests
    {
        private readonly IPoliceClient _client = Substitute.For<IPoliceClient>();
        private readonly AlloggiatiServiceSession _session;

        public AlloggiatiServiceSessionTests()
        {
            var structureService = Substitute.For<IStructureService>();
            structureService.GetAlloggiatiServiceCredentials().Returns(new Credentials { Utente = "utente", Password = "pwd", WsKey = "key" });
            _client.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns(TokenResponse(true, "the-token"));
            _client.TestAsync(Arg.Any<TestRequest>()).Returns(new TestResponse { Body = new TestResponseBody { TestResult = Ok(), Result = Details(true) } });
            _client.SendAsync(Arg.Any<SendRequest>()).Returns(new SendResponse { Body = new SendResponseBody { SendResult = Ok(), Result = Details(true) } });
            _session = new AlloggiatiServiceSession(structureService, _client);
        }

        private static EsitoOperazioneServizio Ok() => new() { Esito = true };

        private static EsitoOperazioneServizio Error(string message) => new() { Esito = false, ErroreDettaglio = message };

        private static ElencoSchedineEsito Details(params bool[] results)
            => new() { Dettaglio = results.Select((ok, i) => new EsitoOperazioneServizio { Esito = ok, ErroreDettaglio = $"record {i}" }).ToArray() };

        private static GenerateTokenResponse TokenResponse(bool ok, string token) => new()
        {
            Body = new GenerateTokenResponseBody
            {
                Result = ok ? Ok() : Error("wrong credentials"),
                GenerateTokenResult = new TokenInfo { Token = token },
            },
        };

        private static async Task<BookingCalendarException> Thrown(Func<Task> act) => (await act.Should().ThrowAsync<BookingCalendarException>()).Which;

        [Fact]
        public async Task Open_generates_a_token_with_the_structure_credentials()
        {
            await _session.Open();

            await _client.Received(1).GenerateTokenAsync(Arg.Is<GenerateTokenRequest>(r =>
                r.Body.Utente == "utente" && r.Body.Password == "pwd" && r.Body.WsKey == "key"));
        }

        [Fact]
        public async Task Open_fails_when_the_service_refuses_the_credentials()
        {
            _client.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns(TokenResponse(false, ""));

            var exception = await Thrown(() => _session.Open());

            exception.ErrorCode.Should().Be(BCError.POLICE_SERVICE_ERROR);
            exception.Message.Should().Be("wrong credentials");
        }

        [Fact]
        public async Task Open_maps_communication_failures()
        {
            _client.GenerateTokenAsync(Arg.Any<GenerateTokenRequest>()).Returns<GenerateTokenResponse>(_ => throw new CommunicationException("offline"));

            var exception = await Thrown(() => _session.Open());

            exception.ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
            exception.Message.Should().Contain("offline");
        }

        [Fact]
        public async Task SendData_in_test_mode_tests_the_records_with_the_session_token()
        {
            await _session.Open();

            await _session.SendData(new List<string> { "r1", "r2" }, test: true);

            await _client.Received(1).TestAsync(Arg.Is<TestRequest>(r =>
                r.Body.Utente == "utente" && r.Body.Token == "the-token" && r.Body.ElencoSchedine.SequenceEqual(new[] { "r1", "r2" })));
            await _client.DidNotReceiveWithAnyArgs().SendAsync(default!);
        }

        [Fact]
        public async Task SendData_sends_the_records_with_the_session_token()
        {
            await _session.Open();

            await _session.SendData(new List<string> { "r1" }, test: false);

            await _client.Received(1).SendAsync(Arg.Is<SendRequest>(r =>
                r.Body.Utente == "utente" && r.Body.Token == "the-token" && r.Body.ElencoSchedine.SequenceEqual(new[] { "r1" })));
            await _client.DidNotReceiveWithAnyArgs().TestAsync(default!);
        }

        [Fact]
        public async Task SendData_in_test_mode_fails_on_a_failed_operation()
        {
            _client.TestAsync(Arg.Any<TestRequest>()).Returns(new TestResponse { Body = new TestResponseBody { TestResult = Error("bad token"), Result = Details(true) } });

            var exception = await Thrown(() => _session.SendData(new List<string> { "r1" }, true));

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "bad token"));
        }

        [Fact]
        public async Task SendData_in_test_mode_fails_on_a_rejected_record()
        {
            _client.TestAsync(Arg.Any<TestRequest>()).Returns(new TestResponse { Body = new TestResponseBody { TestResult = Ok(), Result = Details(true, false) } });

            var exception = await Thrown(() => _session.SendData(new List<string> { "r1", "r2" }, true));

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "record 1"));
        }

        [Fact]
        public async Task SendData_fails_on_a_failed_operation()
        {
            _client.SendAsync(Arg.Any<SendRequest>()).Returns(new SendResponse { Body = new SendResponseBody { SendResult = Error("bad token"), Result = Details(true) } });

            var exception = await Thrown(() => _session.SendData(new List<string> { "r1" }, false));

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "bad token"));
        }

        [Fact]
        public async Task SendData_fails_on_a_rejected_record()
        {
            _client.SendAsync(Arg.Any<SendRequest>()).Returns(new SendResponse { Body = new SendResponseBody { SendResult = Ok(), Result = Details(false, true) } });

            var exception = await Thrown(() => _session.SendData(new List<string> { "r1", "r2" }, false));

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "record 0"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SendData_maps_communication_failures(bool test)
        {
            _client.TestAsync(Arg.Any<TestRequest>()).Returns<TestResponse>(_ => throw new CommunicationException("offline"));
            _client.SendAsync(Arg.Any<SendRequest>()).Returns<SendResponse>(_ => throw new CommunicationException("offline"));

            (await Thrown(() => _session.SendData(new List<string> { "r1" }, test))).ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
        }

        [Fact]
        public async Task GetRicevuta_returns_the_pdf_of_the_day()
        {
            var pdf = new byte[] { 1, 2, 3 };
            _client.RicevutaAsync(Arg.Any<RicevutaRequest>()).Returns(new RicevutaResponse { Body = new RicevutaResponseBody { RicevutaResult = Ok(), PDF = pdf } });
            await _session.Open();

            var result = await _session.GetRicevuta(new DateTime(2023, 6, 10));

            result.Should().BeSameAs(pdf);
            await _client.Received(1).RicevutaAsync(Arg.Is<RicevutaRequest>(r =>
                r.Body.Utente == "utente" && r.Body.Token == "the-token" && r.Body.Data == new DateTime(2023, 6, 10)));
        }

        [Fact]
        public async Task GetRicevuta_fails_on_a_failed_operation()
        {
            _client.RicevutaAsync(Arg.Any<RicevutaRequest>()).Returns(new RicevutaResponse { Body = new RicevutaResponseBody { RicevutaResult = Error("no data") } });

            var exception = await Thrown(() => _session.GetRicevuta(DateTime.Today));

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "no data"));
        }

        [Fact]
        public async Task GetRicevuta_maps_communication_failures()
        {
            _client.RicevutaAsync(Arg.Any<RicevutaRequest>()).Returns<RicevutaResponse>(_ => throw new CommunicationException("offline"));

            (await Thrown(() => _session.GetRicevuta(DateTime.Today))).ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
        }

        [Fact]
        public async Task GetPlaces_parses_the_places_table()
        {
            _client.TabellaAsync(Arg.Any<TabellaRequest>()).Returns(new TabellaResponse
            {
                Body = new TabellaResponseBody
                {
                    TabellaResult = Ok(),
                    CSV = "Codice;Descrizione;Provincia;DataFineVal\n412058091;ROMA;RM;\n403015146;MILANO;MI;31/12/2099\n",
                },
            });
            await _session.Open();

            var places = await _session.GetPlaces();

            places.Select(p => (p.Codice, p.Descrizione, p.Provincia)).Should().Equal((412058091L, "ROMA", "RM"), (403015146L, "MILANO", "MI"));
            await _client.Received(1).TabellaAsync(Arg.Is<TabellaRequest>(r =>
                r.Body.Utente == "utente" && r.Body.Token == "the-token" && r.Body.Tipo == TipoTabella.Luoghi));
        }

        [Fact]
        public async Task GetPlaces_fails_on_a_failed_operation()
        {
            _client.TabellaAsync(Arg.Any<TabellaRequest>()).Returns(new TabellaResponse { Body = new TabellaResponseBody { TabellaResult = Error("expired") } });

            var exception = await Thrown(() => _session.GetPlaces());

            (exception.ErrorCode, exception.Message).Should().Be((BCError.POLICE_SERVICE_ERROR, "expired"));
        }

        [Fact]
        public async Task GetPlaces_maps_communication_failures()
        {
            _client.TabellaAsync(Arg.Any<TabellaRequest>()).Returns<TabellaResponse>(_ => throw new CommunicationException("offline"));

            (await Thrown(() => _session.GetPlaces())).ErrorCode.Should().Be(BCError.CONNECTION_ERROR);
        }
    }
}
