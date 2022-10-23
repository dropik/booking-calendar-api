namespace BookingCalendarApi.Models.AlloggiatiService
{
    public class Place
    {
        public ulong Codice { get; set; }
        public string Descrizione { get; set; } = "";
        public string Provincia { get; set; } = "";
        public DateTime? DataFineVal { get; set; }
    }
}
