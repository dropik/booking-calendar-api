namespace BookingCalendarApi.Models.Clients.Police
{
    public class RicevutaResponse
    {
        public RicevutaResponseBody Body { get; set; }
    }

    public class RicevutaResponseBody
    {
        public EsitoOperazioneServizio RicevutaResult { get; set; }
        public byte[] PDF { get; set; }
    }
}
