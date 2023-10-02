namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class AssignedRoom<TGuest> : BookingRoom<TGuest>
    {
        public AssignedRoom(long stayId, string roomName, string arrival, string departure, string rateId)
            : base(stayId, roomName, arrival, departure, rateId) { }

        public long? RoomId { get; set; }
    }
}
