namespace BookingCalendarApi.Repository
{
    public class Nation
    {
        public Nation(string iso, ulong code, string description)
        {
            Iso = iso;
            Code = code;
            Description = description;
        }

        public string Iso { get; set; } = "";
        public ulong Code { get; set; }
        public string Description { get; set; } = "";
    }
}
