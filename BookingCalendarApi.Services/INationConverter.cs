namespace BookingCalendarApi.Services
{
    public interface INationConverter
    {
        ulong GetCodeByIso(string iso);
    }
}
