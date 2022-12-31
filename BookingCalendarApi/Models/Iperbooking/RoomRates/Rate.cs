namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class Rate
    {
        public Rate(string rateId, uint minOccupancy, uint maxOccupancy, string baseBoard)
        {
            RateId = rateId;
            MinOccupancy = minOccupancy;
            MaxOccupancy = maxOccupancy;
            BaseBoard = baseBoard;
        }

        public string RateId { get; set; } = "";
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }
        public string BaseBoard { get; set; } = "";
    }
}
