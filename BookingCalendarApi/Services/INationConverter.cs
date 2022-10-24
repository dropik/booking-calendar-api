namespace BookingCalendarApi.Services
{
    public interface INationConverter
    {
        public ulong GetCodeByIso(string iso);
    }
}
