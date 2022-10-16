namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class Rate
    {
        public Rate(uint minOccupancy, uint maxOccupancy)
        {
            MinOccupancy = minOccupancy;
            MaxOccupancy = maxOccupancy;
        }
        
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }
    }
}
