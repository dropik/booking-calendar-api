namespace BookingCalendarApi.Models.AlloggiatiService
{
    public class Place
    {
        public long Codice { get; set; }
        public string Descrizione { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string DataFineVal { get; set; }
    }
}
