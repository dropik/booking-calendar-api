using AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface IAlloggiatiServiceSession
    {
        public Task OpenAsync();
        public Task<string> GetTableAsync(TipoTabella tipoTabella);
        public Task SendDataAsync(IList<string> data, bool test);
    }
}
