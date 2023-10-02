namespace BookingCalendarApi.Models.Configurations
{
    public class JWT
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }

        public const string REFRESH_TOKEN_CLAIM = "RTId";
        public const string STRUCTURE_CLAIM = "StructureId";
    }
}
