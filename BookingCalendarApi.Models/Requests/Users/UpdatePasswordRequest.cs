namespace BookingCalendarApi.Models.Requests.Users
{
    public class UpdatePasswordRequest
    {
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}
