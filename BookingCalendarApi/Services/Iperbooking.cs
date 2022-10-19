using BookingCalendarApi.Models.Iperbooking;
using System.Text;
using System.Text.Json;

namespace BookingCalendarApi.Services
{
    public class Iperbooking : IIperbooking
    {
        private readonly Auth _auth;

        public Iperbooking(IConfiguration configuration)
        {
            _auth = configuration.GetSection("Iperbooking").Get<Auth>();
        }

        public async Task<IEnumerable<Models.Iperbooking.RoomRates.Room>> GetRoomRatesAsync()
        {
            var url = $"https://api.iperbooking.net/v1/GetRoomRates.cfm?idhotel={_auth.IdHotel}&username={_auth.Username}&password={_auth.Password}&format=json";
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
                Console.WriteLine(exception.Message);
            }

            return new List<Models.Iperbooking.RoomRates.Room>();
        }

        public async Task<IEnumerable<Models.Iperbooking.Bookings.Booking>> GetBookingsAsync(string arrivalFrom, string arrivalTo)
        {
            var url = $"https://api.iperbooking.net/v1/GetBookings.cfm?idhotel={_auth.IdHotel}&username={_auth.Username}&password={_auth.Password}&format=json&arrivalfrom={arrivalFrom}&arrivalto={arrivalTo}";
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
                Console.WriteLine(exception.Message);
            }

            return new List<Models.Iperbooking.Bookings.Booking>();
        }

        public async Task<IEnumerable<Models.Iperbooking.Guests.GuestsResponse>> GetGuestsAsync(string reservationId)
        {
            try
            {
                var requestData = new Models.Iperbooking.Guests.GuestsRequest(int.Parse(_auth.IdHotel), _auth.Username, _auth.Password, reservationId);
                var url = "https://api.iperbooking.net/v1/GetGuests.cfm";
                
                using HttpClient client = new();
                using var response = await client.PostAsJsonAsync(url, requestData, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                using var content = response.Content;
                var data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    var poco = JsonSerializer.Deserialize<ICollection<Models.Iperbooking.Guests.GuestsResponse>>(data, new JsonSerializerOptions()
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
                }
                else
                {
                    throw new Exception("Fetched empty data from iperbooking");
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new List<Models.Iperbooking.Guests.GuestsResponse>();
        }
    }
}
