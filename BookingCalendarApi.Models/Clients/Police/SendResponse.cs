namespace BookingCalendarApi.Models.Clients.Police
{
    public class SendResponse
    {
        public SendResponseBody Body { get; set; }
    }

    public class SendResponseBody
    {
        public EsitoOperazioneServizio SendResult { get; set; }
        public ElencoSchedineEsito Result { get; set; }
    }
}
