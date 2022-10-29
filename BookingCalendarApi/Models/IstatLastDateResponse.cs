namespace BookingCalendarApi.Models
{
    public class IstatLastDateResponse
    {
        public IstatLastDateResponse(string lastDate)
        {
            LastDate = lastDate;
        }

        public string LastDate { get; set; }
    }
}
