using System;

namespace BookingCalendarApi.Models.Clients.Police
{
    public class RicevutaRequest
    {
        public RicevutaRequestBody Body { get; set; }
    }

    public class RicevutaRequestBody
    {
        public string Utente { get; set; }
        public string Token { get; set; }
        public DateTime Data { get; set; }
    }
}
