using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class NationConverterProvider : INationConverterProvider
    {
        private readonly Func<IEnumerable<PoliceNationCode>, INationConverter> _nationConverterProvider;
        private readonly BookingCalendarContext _context;

        public NationConverterProvider(Func<IEnumerable<PoliceNationCode>, INationConverter> nationConverterProvider, BookingCalendarContext context)
        {
            _nationConverterProvider = nationConverterProvider;
            Converter = nationConverterProvider(new List<PoliceNationCode>());
            _context = context;
        }

        public INationConverter Converter { get; private set; }

        public async Task FetchAsync()
        {
            var policeNations = await _context.PoliceNations.ToListAsync();
            Converter = _nationConverterProvider(policeNations);
        }
    }
}
