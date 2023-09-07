namespace BookingCalendarApi.Models.Clients.Police
{
    public class TabellaRequest
    {
        public TabellaRequestBody Body { get; set; }
    }

    public class TabellaRequestBody
    {
        public string Utente { get; set; }
        public string Token { get; set; }
        public TipoTabella Tipo { get; set; }
    }

    public enum TipoTabella
    {
        Luoghi = 0,
        Tipi_Documento = 1,
        Tipi_Alloggiato = 2,
        TipoErrore = 3,
        ListaAppartamenti = 4,
    }
}
