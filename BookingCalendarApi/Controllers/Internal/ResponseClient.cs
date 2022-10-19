namespace BookingCalendarApi.Controllers.Internal
{
    public class ResponseClient
    {
        public ResponseClient(string id, string bookingId, string name, string surname, string dateOfBirth)
        {
            Id = id;
            BookingId = bookingId;
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
        }

        public string Id { get; set; }
        public string BookingId { get; set;}
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? StateOfBirth { get; set; }
    }
}
