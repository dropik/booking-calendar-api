using BookingCalendarApi.Models.AlloggiatiService;
using System.Collections.Generic;
using System.Linq;

namespace BookingCalendarApi.Services
{
    public class PlaceConverter : IPlaceConverter
    {
        private readonly List<Place> _places;

        public PlaceConverter(DataContext context)
        {
            _places = context.Places;
        }

        public ulong? GetPlaceCodeByDescription(string description)
        {
            var formattedDescriptionQuery = description
                .Select(c =>
                {
                    switch (c)
                    {
                        case 'à': return "A'";
                        case 'ò': return "O'";
                        case 'è': return "E'";
                        case 'é': return "E'";
                        case 'ù': return "U'";
                        case 'ì': return "I'";
                        default: return $"{char.ToUpper(c)}";
                    }
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
