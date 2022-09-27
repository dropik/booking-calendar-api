using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IEnumerable<KeyValuePair<string, IEnumerable<uint>>>> GetAsync()
        {
            var roomTypes = new Dictionary<string, IEnumerable<uint>>();
            var roomRates = await iperbooking.GetRoomRates();
            foreach (var roomRate in roomRates)
            {
                var roomName = roomRate.RoomName;
                var acceptedOccupations = new HashSet<uint>();
                try
                {
                    if (roomRate.RateGroups.Count > 0)
                    {
                        var rateGroup = roomRate.RateGroups.ElementAt(0);
                        if (rateGroup.Rates.Count > 0)
                        {
                            var rate = rateGroup.Rates.ElementAt(0);
                            var minOccupancy = rate.MinOccupancy;
                            var maxOccupancy = rate.MaxOccupancy;

                            for (var i = minOccupancy; i <= maxOccupancy; i++)
                            {
                                acceptedOccupations.Add(i);
                            }
                        } else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                } catch (Exception)
                {
                    continue;
                }

                roomTypes.Add(roomName, acceptedOccupations);
            }
            return roomTypes;
        }
    }
}
