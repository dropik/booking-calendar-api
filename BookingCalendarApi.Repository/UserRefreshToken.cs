using System;

namespace BookingCalendarApi.Repository
{
    public class UserRefreshToken
    {
        public long Id { get; set; }
        public string RefreshToken { get; set; } = "";
        public string Username { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}
