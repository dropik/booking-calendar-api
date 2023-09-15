namespace BookingCalendarApi.Models.Clients.Police
{
    public class TestResponse
    {
        public TestResponseBody Body { get; set; }
    }

    public class TestResponseBody
    {
        public EsitoOperazioneServizio TestResult { get; set; }
        public ElencoSchedineEsito Result { get; set; }
    }
}
