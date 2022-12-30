namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class BookingGuest
    {
        public BookingGuest(bool isChild)
        {
            IsChild = isChild;
        }
        
        public bool IsChild { get; set; }
    }
}
