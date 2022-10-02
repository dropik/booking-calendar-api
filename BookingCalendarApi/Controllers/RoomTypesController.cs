using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;
using RoomRate = BookingCalendarApi.Models.Iperbooking.RoomRates.Room;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-types")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly IIperbooking iperbooking;

        public RoomTypesController(IIperbooking iperbooking)
        {
            this.iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<IEnumerable<RoomType>> GetAsync()
        {
            var roomRates = await iperbooking.GetRoomRatesAsync();
            return ConvertRoomRatesToRoomTypes(roomRates);
        }

        private IEnumerable<RoomType> ConvertRoomRatesToRoomTypes(ICollection<RoomRate> roomRates)
        {
            foreach (var roomRate in roomRates)
            {
                var newType = new RoomType()
                {
                    Name = roomRate.RoomName
                };

                if (roomRate.RateGroups.Count > 0)
                {
                    var rateGroup = roomRate.RateGroups.ElementAt(0);
                    if (rateGroup.Rates.Count > 0)
                    {
                        var rate = rateGroup.Rates.ElementAt(0);
                        newType.MinOccupancy = rate.MinOccupancy;
                        newType.MaxOccupancy = rate.MaxOccupancy;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                yield return newType;
            }
        }
    }
}
