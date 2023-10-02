namespace BookingCalendarApi.Models.Responses
{
    public class ClientResponse
    {
        public string Id { get; set; }
        public string BookingId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; } = null;
        public string ProvinceOfBirth { get; set; } = null;
        public string StateOfBirth { get; set; } = null;

        public ClientResponse(string id, string bookingId, string name, string surname, string dateOfBirth)
        {
            Id = id;
            BookingId = bookingId;
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
        }
    }
}
