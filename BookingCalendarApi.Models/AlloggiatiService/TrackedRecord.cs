using BookingCalendarApi.Models.Exceptions;
using System;
using System.Linq;

namespace BookingCalendarApi.Models.AlloggiatiService
{
    public class TrackedRecord
    {
        private ushort _nights;
        private string _surname = "";
        private string _name = "";
        private DocumentType? _documentType;
        private string _documentNumber;
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
                    throw new BookingCalendarException(BCError.MAX_STAY_EXCEEDED, "Exceeded maximum stay of 30 nights");
                }

                _nights = value;
            }
        }
        public string Surname
        {
            get => _surname;
            set => _surname = GetSanitizedName(value);
        }
        public string Name
        {
            get => _name;
            set => _name = GetSanitizedName(value);
        }
        public Gender Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public ulong? PlaceOfBirth { get; set; }
        public string ProvinceOfBirth { get; set; }
        public ulong StateOfBirth { get; set; }
        public ulong Citizenship { get; set; }
        public DocumentType? DocType
        {
            get => _documentType;
            set => _documentType = SetIfNotMember(value);
        }
        public string DocNumber
        {
            get => _documentNumber;
            set => _documentNumber = SetIfNotMember(value);
        }
        public ulong? DocIssuer
        {
            get => _documentIssuer;
            set => _documentIssuer = SetIfNotMember(value);
        }

        private T SetIfNotMember<T>(T value)
        {
            return Type == AccomodatedType.FamilyMember || Type == AccomodatedType.GroupMember ? default : value;
        }

        private static string GetSanitizedName(string value)
        {
            var valueTransformationQuery = value
                    .Select(c =>
                    {
                        switch (c)
                        {
                            case 'à': return "A'";
                            case 'ò': return "O'";
                            case 'è': return "E'";
                            case 'é': return "E'";
                            case 'ù': return "U'";
                            case 'ì': return "I'";
                            default: return $"{char.ToUpper(c)}";
                        }
                    })
                    .SelectMany(s => s);

            var result = "";
            foreach (var c in valueTransformationQuery)
            {
                result += c;
            }
            return result;
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

        public enum DocumentType
        {
            Ident,
            Pasor,
            Paten,
            Idele,
        }
    }
}
