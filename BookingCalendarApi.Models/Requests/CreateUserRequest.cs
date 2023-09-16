﻿namespace BookingCalendarApi.Models.Requests
{
    public class CreateUserRequest
    {
        public int StructureId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VisibleName { get; set; }
    }
}
