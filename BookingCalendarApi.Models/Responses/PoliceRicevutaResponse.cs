using System;

namespace BookingCalendarApi.Models.Responses
{
    public class PoliceRicevutaResponse
    {
        public byte[] Pdf { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "";
    }
}
