namespace BookingCalendarApi.Models.Configurations
{
    public class JWT
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public int AccessTokenExpirationMinutes { get; set; }
    }
}
