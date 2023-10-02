using System.Collections.Generic;

namespace BookingCalendarApi.Models.Clients.Police
{
    public class SendRequest
    {
        public SendRequestBody Body { get; set; }
    }

    public class SendRequestBody
    {
        public string Utente { get; set; }
        public string Token { get; set; }
        public List<string> ElencoSchedine { get; set; } = new List<string>();
    }
}
