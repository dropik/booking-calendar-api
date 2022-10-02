using BookingCalendarApi.Models.Iperbooking;
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

        public async Task<ICollection<Models.Iperbooking.RoomRates.Room>> GetRoomRatesAsync()
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
                    var poco = JsonSerializer.Deserialize<ICollection<Models.Iperbooking.RoomRates.Room>>(data, new JsonSerializerOptions()
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

            return new HashSet<Models.Iperbooking.RoomRates.Room>();
        }

        public async Task<ICollection<Models.Iperbooking.Bookings.Booking>> GetBookingsAsync(string arrivalFrom, string arrivalTo)
        {
            var url = $"https://api.iperbooking.net/v1/GetBookings.cfm?idhotel={auth.IdHotel}&username={auth.Username}&password={auth.Password}&format=json&arrivalfrom={arrivalFrom}&arrivalto={arrivalTo}";
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;

                var data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    var poco = JsonSerializer.Deserialize<ICollection<Models.Iperbooking.Bookings.Booking>>(data, new JsonSerializerOptions()
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

            return new HashSet<Models.Iperbooking.Bookings.Booking>();
        }
    }
}
