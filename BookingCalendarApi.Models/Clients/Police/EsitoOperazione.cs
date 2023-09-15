namespace BookingCalendarApi.Models.Clients.Police
{
    public class EsitoOperazione
    {
        public bool Esito { get; set; }
    }

    public class EsitoOperazioneServizio : EsitoOperazione
    {
        public string ErroreDettaglio { get; set; }
    }

    public class ElencoSchedineEsito
    {
        public EsitoOperazioneServizio[] Dettaglio { get; set; }
    }
}
