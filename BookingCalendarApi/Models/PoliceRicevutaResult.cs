namespace BookingCalendarApi.Models
{
    public class PoliceRicevutaResult
    {
        public byte[] Pdf { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "";
    }
}
