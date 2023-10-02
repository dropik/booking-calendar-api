namespace BookingCalendarApi.Models.Clients.Police
{
    public class TabellaResponse
    {
        public TabellaResponseBody Body { get; set; }
    }

    public class TabellaResponseBody
    {
        public EsitoOperazioneServizio TabellaResult { get; set; }
        public string CSV { get; set; }
    }
}
