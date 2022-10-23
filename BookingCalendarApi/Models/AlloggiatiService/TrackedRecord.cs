namespace BookingCalendarApi.Models.AlloggiatiService
{
    public class TrackedRecord
    {
        private ushort _nights;
        private string? _provinceOfBirth;
        private string? _documentType;
        private string? _documentNumber;
        private ulong? _documentIssuer;

        public AccomodatedType Type { get; set; }
        public DateTime Arrival { get; set; }
        public ushort Nights
        {
            get => _nights;
            set
            {
                if (value > 30)
                {
                    throw new Exception("Exceeded maximum stay of 30 nights");
                }

                _nights = value;
            }
        }
        public string Surname { get; set; } = "";
        public string Name { get; set; } = "";
        public Gender Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public ulong? PlaceOfBirth { get; set; }
        public string? ProvinceOfBirth
        {
            get => _provinceOfBirth;
            set
            {
                if (value != null && value.Length != 2)
                {
                    throw new Exception("Incorrect province value");
                }
                _provinceOfBirth = value;
            }
        }
        public ulong StateOfBirth { get; set; }
        public ulong Citizenship { get; set; }
        public string? DocumentType
        {
            get => _documentType;
            set => _documentType = SetIfNotMember(value);
        }
        public string? DocumentNumber
        {
            get => _documentNumber;
            set => _documentNumber = SetIfNotMember(value);
        }
        public ulong? DocumentIssuer
        {
            get => _documentIssuer;
            set => _documentIssuer = SetIfNotMember(value);
        }

        private T? SetIfNotMember<T>(T? value)
        {
            return Type == AccomodatedType.FamilyMember || Type == AccomodatedType.GroupMember ? default : value;
        }

        public enum Gender
        {
            Male,
            Female
        }

        public enum AccomodatedType
        {
            SingleGuest,
            FamilyHead,
            GroupHead,
            FamilyMember,
            GroupMember,
        }
    }
}
