using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public class PlaceConverter : IPlaceConverter
    {
        private readonly IEnumerable<Place> _places;

        public PlaceConverter(IEnumerable<Place> places)
        {
            _places = places;
        }

        public ulong? GetPlaceCodeByDescription(string description)
        {
            var formattedDescriptionQuery = description
                .Select(c => c switch
                {
                    'à' => "A'",
                    'ò' => "O'",
                    'è' => "E'",
                    'é' => "E'",
                    'ù' => "U'",
                    'ì' => "I'",
                    _ => $"{char.ToUpper(c)}"
                })
                .SelectMany(s => s);

            var formattedDescription = "";
            foreach (var c in formattedDescriptionQuery)
            {
                formattedDescription += c;
            }

            var foundPlaceEntry = _places
                .SingleOrDefault(place => place.Descrizione == formattedDescription);

            return foundPlaceEntry?.Codice;
        }
    }
}
