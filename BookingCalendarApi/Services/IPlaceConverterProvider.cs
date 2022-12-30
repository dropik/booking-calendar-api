namespace BookingCalendarApi.Services
{
    public interface IPlaceConverterProvider
    {
        Task FetchAsync();
        IPlaceConverter Converter { get; }
    }
}
