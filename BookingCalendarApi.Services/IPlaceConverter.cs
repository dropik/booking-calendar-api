namespace BookingCalendarApi.Services
{
    public interface IPlaceConverter
    {
        ulong? GetPlaceCodeByDescription(string description);
    }
}
