namespace BookingCalendarApi.Repository
{
    public class Nation
    {
        public string Iso { get; set; } = "";
        public long Code { get; set; }
        public string Description { get; set; } = "";

        public Nation() { }

        public Nation(string iso, long code, string description)
        {
            Iso = iso;
            Code = code;
            Description = description;
        }
    }
}
