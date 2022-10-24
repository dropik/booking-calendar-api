namespace BookingCalendarApi.Services
{
    public interface INationConverterProvider
    {
        public Task FetchAsync();
        public INationConverter Converter { get; }
    }
}
