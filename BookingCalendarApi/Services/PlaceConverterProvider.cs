using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public class PlaceConverterProvider : IPlaceConverterProvider
    {
        private readonly IAlloggiatiTableReader _tableReader;
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;

        public PlaceConverterProvider(IAlloggiatiTableReader tableReader, Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider)
        {
            _tableReader = tableReader;
            _placeConverterProvider = placeConverterProvider;
            Converter = _placeConverterProvider(new List<Place>());
        }

        public IPlaceConverter Converter { get; private set; }

        public async Task FetchAsync(IAlloggiatiServiceSession session)
        {
            await session.OpenAsync();
            var placesStr = await session.GetTableAsync(AlloggiatiService.TipoTabella.Luoghi);
            var places = _tableReader.ReadAsPlaces(placesStr);
            Converter = _placeConverterProvider(places);
        }
    }
}
