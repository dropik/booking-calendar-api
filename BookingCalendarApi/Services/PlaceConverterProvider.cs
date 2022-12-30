using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public class PlaceConverterProvider : IPlaceConverterProvider
    {
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;

        public PlaceConverterProvider(Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider)
        {
            _placeConverterProvider = placeConverterProvider;
            Converter = _placeConverterProvider(new List<Place>());
        }

        public IPlaceConverter Converter { get; private set; }

        public async Task FetchAsync(IAlloggiatiServiceSession session)
        {
            await session.OpenAsync();
            var places = await session.GetPlacesAsync(AlloggiatiService.TipoTabella.Luoghi);
            Converter = _placeConverterProvider(places);
        }
    }
}
