namespace BookingCalendarApi.Services
{
    public interface IPlaceConverterProvider
    {
        public Task FetchAsync(IAlloggiatiServiceSession session);
        public IPlaceConverter Converter { get; }
    }
}
