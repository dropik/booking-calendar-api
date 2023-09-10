namespace BookingCalendarApi.Services
{
    public interface INationConverter
    {
        long GetCodeByIso(string iso);
    }
}
