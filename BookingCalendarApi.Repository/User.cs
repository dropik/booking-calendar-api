﻿using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class User
    {
        public const int MASTER_ID = -1;
        public const string ADMIN_ROLE = "admin";

        public long Id { get; set; }
        public long StructureId { get; set; }
        [JsonIgnore]
        public Structure Structure { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string VisibleName { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
}
