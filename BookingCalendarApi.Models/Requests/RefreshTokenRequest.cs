﻿namespace BookingCalendarApi.Models.Requests
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
