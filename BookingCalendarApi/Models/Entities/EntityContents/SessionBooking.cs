namespace BookingCalendarApi.Models.Entities.EntityContents
{
    public class SessionBooking
    {
        public SessionBooking(string bookingId, string lastModified)
        {
            BookingId = bookingId;
            LastModified = lastModified;
        }

        public string BookingId { get; set; }
        public string LastModified { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SessionBooking tile &&
                   BookingId == tile.BookingId &&
                   LastModified == tile.LastModified;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BookingId, LastModified);
        }
    }
}
