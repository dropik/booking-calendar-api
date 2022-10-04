namespace BookingCalendarApi.Models.Iperbooking
{
    public class Auth
    {
        public Auth(string idHotel, string username, string password)
        {
            IdHotel = idHotel;
            Username = username;
            Password = password;
        }
        
        public string IdHotel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
