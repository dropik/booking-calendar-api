using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public class PlaceConverterProvider : IPlaceConverterProvider
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;

        public PlaceConverterProvider(IAlloggiatiServiceSession session, Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider)
        {
            _session = session;
            _placeConverterProvider = placeConverterProvider;
            Converter = _placeConverterProvider(new List<Place>());
        }

        public IPlaceConverter Converter { get; private set; }

        public async Task FetchAsync()
        {
            var places = await _session.GetPlacesAsync(AlloggiatiService.TipoTabella.Luoghi);
            Converter = _placeConverterProvider(places);
        }
    }
}
