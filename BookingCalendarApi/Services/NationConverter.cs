using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class NationConverter : INationConverter
    {
        private readonly IEnumerable<PoliceNationCode> _nationCodes;

        public NationConverter(IEnumerable<PoliceNationCode> nationCodes)
        {
            _nationCodes = nationCodes;
        }

        public ulong GetCodeByIso(string iso)
        {
            var foundCodeEntry = _nationCodes
                .SingleOrDefault(nationCode => nationCode.Iso == iso);

            if (foundCodeEntry == null)
            {
                throw new Exception("Cannot find nation by ISO code");
            }

            return foundCodeEntry.Code;
        }
    }
}
