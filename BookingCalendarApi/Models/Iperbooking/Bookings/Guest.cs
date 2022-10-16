namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Guest
    {
        public Guest(bool isChild)
        {
            IsChild = isChild;
        }
        
        public bool IsChild { get; set; }
    }
}
