using System;

namespace BookingCalendarApi.Models
{
    public class UserRefreshToken
    {
        public string RefreshToken { get; set; } = "";
        public string Username { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}
