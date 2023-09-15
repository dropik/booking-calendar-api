using System.Collections.Generic;

namespace BookingCalendarApi.Models.Clients.Police
{
    public class TestRequest
    {
        public TestRequestBody Body { get; set; }
    }

    public class TestRequestBody
    {
        public string Utente { get; set; }
        public string Token { get; set; }
        public List<string> ElencoSchedine { get; set; } = new List<string>();
    }
}
