namespace BookingCalendarApi.Models.Clients.Police
{
    public class GenerateTokenResponse
    {
        public GenerateTokenResponseBody Body { get; set; }
    }

    public class GenerateTokenResponseBody
    {
        public TokenInfo GenerateTokenResult { get; set; }
        public EsitoOperazioneServizio Result { get; set; }
    }

    public class TokenInfo
    {
        public string Token { get; set; }
    }
}
