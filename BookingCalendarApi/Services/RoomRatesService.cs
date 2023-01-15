using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public class RoomRatesService : IRoomRatesService
    {
        private readonly IIperbooking _iperbooking;

        public RoomRatesService(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        public async Task<RoomRatesResponse> Get()
        {
            var roomRates = await _iperbooking.GetRoomRatesAsync();
            return new RoomRatesResponse()
            {
                RoomTypes = roomRates
                    .SelectMany(
                        roomRate => roomRate.RateGroups.Take(1),
                        (roomRate, rateGroup) => new { name = roomRate.RoomName, rateGroup })
                    .SelectMany(
                        roomRate => roomRate.rateGroup.Rates.Take(1),
                        (roomRate, rate) => new RoomType(roomRate.name, rate.MinOccupancy, rate.MaxOccupancy))
                    .ToList(),

                RoomRates = roomRates
                    .SelectMany(rate => rate.RateGroups)
                    .SelectMany(group => group.Rates)
                    .ToList()
            };
        }
    }
}
