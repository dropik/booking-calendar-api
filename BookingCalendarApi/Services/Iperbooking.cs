using BookingCalendarApi.Models.Iperbooking;

namespace BookingCalendarApi.Services
{
    public class Iperbooking : IIperbooking
    {
        private readonly Auth auth;

        public Iperbooking(IConfiguration configuration)
        {
            auth = configuration.GetSection("Iperbooking").Get<Auth>();
        }

        public async Task<ICollection<Room>> GetRoomRates()
        {
            throw new NotImplementedException();
        }
    }
}
