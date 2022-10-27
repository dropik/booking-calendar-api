using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class NationConverter : INationConverter
    {
        private readonly IEnumerable<Nation> _nations;

        public NationConverter(IEnumerable<Nation> nations)
        {
            _nations = nations;
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
