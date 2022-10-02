using BookingCalendarApi.Models.Iperbooking;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using System.Text.Json;

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
            var url = $"https://api.iperbooking.net/v1/GetRoomRates.cfm?idhotel={auth.IdHotel}&username={auth.Username}&password={auth.Password}&format=json";
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;

                var data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    var poco = JsonSerializer.Deserialize<ICollection<Room>>(data, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (poco != null)
                    {
                        return poco;
                    } else
                    {
                        throw new Exception("Cannot deserialize fetched iperbooking data");
                    }
                } else
                {
                    throw new Exception("Fetched empty data from iperbooking");
                }
            } catch(Exception exception)
            {
                Console.WriteLine(exception);
            }

            return new HashSet<Room>();
        }
    }
}
