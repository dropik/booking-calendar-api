namespace BookingCalendarApi.Models.Clients.Police
{
    public class GenerateTokenRequest
    {
        public GenerateTokenRequestBody Body { get; set; }
    }

    public class GenerateTokenRequestBody
    {
        public string Utente { get; set; }
        public string Password { get; set; }
        public string WsKey { get; set; }
    }
}
