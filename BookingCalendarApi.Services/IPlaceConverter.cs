namespace BookingCalendarApi.Services
{
    public interface IPlaceConverter
    {
        long? GetPlaceCodeByDescription(string description);
    }
}
