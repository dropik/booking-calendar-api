using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class NationConverterProvider : INationConverterProvider
    {
        private readonly Func<IEnumerable<Nation>, INationConverter> _nationConverterProvider;
        private readonly BookingCalendarContext _context;

        public NationConverterProvider(Func<IEnumerable<Nation>, INationConverter> nationConverterProvider, BookingCalendarContext context)
        {
            _nationConverterProvider = nationConverterProvider;
            Converter = nationConverterProvider(new List<Nation>());
            _context = context;
        }

        public INationConverter Converter { get; private set; }

        public async Task FetchAsync()
        {
            var policeNations = await _context.Nations.ToListAsync();
            Converter = _nationConverterProvider(policeNations);
        }
    }
}
