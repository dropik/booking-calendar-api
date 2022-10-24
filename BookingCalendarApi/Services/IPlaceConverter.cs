namespace BookingCalendarApi.Services
{
    public interface IPlaceConverter
    {
        public ulong? GetPlaceCodeByDescription(string description);
    }
}
