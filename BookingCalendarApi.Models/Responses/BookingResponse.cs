using BookingCalendarApi.Models.Json;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace BookingCalendarApi.Models.Responses
{
    public class BookingResponse<TPerson>
    {
        public string Id { get; set; }
        [JsonConverter(typeof(LowerCaseEnumConverter))]
        public BookingStatus Status { get; set; } = BookingStatus.New;
        public string Name { get; set; }
        public string LastModified { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Color { get; set; } = null;
        public bool IsBankTransfer { get; set; }
        public decimal Deposit { get; set; }
        public bool DepositConfirmed { get; set; }

        public List<TileResponse<TPerson>> Tiles { get; set; } = new List<TileResponse<TPerson>>();

        public BookingResponse(
            string id,
            string name,
            string lastModified,
            string from,
            string to,
            decimal deposit,
            bool depositConfirmed,
            bool isBankTransfer)
        {
            Id = id;
            Name = name;
            LastModified = lastModified;
            From = from;
            To = to;
            Deposit = deposit;
            DepositConfirmed = depositConfirmed;
            IsBankTransfer = isBankTransfer;
        }
    }
}
