namespace BookingCalendarApi.Models.AlloggiatiService
{
    public class Credentials
    {
        public Credentials(string utente, string password, string wsKey)
        {
            Utente = utente;
            Password = password;
            WsKey = wsKey;
        }

        public string Utente { get; set; }
        public string Password { get; set; }
        public string WsKey { get; set; }
    }
}
