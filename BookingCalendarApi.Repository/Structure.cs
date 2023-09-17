namespace BookingCalendarApi.Repository
{
    public class Structure
    {
        public const int MASTER_ID = -1;

        public long Id { get; set; }
        public string Name { get; set; } = "";

        // Iperbooking credentials
        public string IperbookingHotel { get; set; } = "";
        public string IperbookingUsername { get; set; } = "";
        public string IperbookingPassword { get; set; } = "";

        // Alloggiati service credentials
        public string ASUtente { get; set; } = "";
        public string ASPassword { get; set; } = "";
        public string ASWsKey { get; set; } = "";

        // C59 credentials
        public string C59Username { get; set; } = "";
        public string C59Password { get; set; } = "";
        public string C59Struttura { get; set; } = "";
    }
}
