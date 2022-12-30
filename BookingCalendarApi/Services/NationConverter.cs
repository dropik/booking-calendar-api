using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class NationConverter : INationConverter
    {
        private readonly List<Nation> _nations;

        public NationConverter(DataContext context)
        {
            _nations = context.Nations;
        }

        public ulong GetCodeByIso(string iso)
        {
            var foundCodeEntry = _nations
                .SingleOrDefault(nation => nation.Iso == iso);

            if (foundCodeEntry == null)
            {
                throw new Exception("Cannot find nation by ISO code");
            }

            return foundCodeEntry.Code;
        }
    }
}
