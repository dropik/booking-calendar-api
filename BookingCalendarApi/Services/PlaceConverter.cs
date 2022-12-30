using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public class PlaceConverter : IPlaceConverter
    {
        private readonly List<Place> _places;

        public PlaceConverter(List<Place> places)
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

            var levenshtein = new Fastenshtein.Levenshtein(formattedDescription);

            var foundPlaceEntry = _places
                .Select(place => new { Place = place, Distance = levenshtein.DistanceFrom(place.Descrizione) })
                .OrderBy(comparison => comparison.Distance)
                .First().Place;

            return foundPlaceEntry?.Codice;
        }
    }
}
