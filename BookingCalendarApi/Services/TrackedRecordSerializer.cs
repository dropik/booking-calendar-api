using BookingCalendarApi.Models.AlloggiatiService;
using static BookingCalendarApi.Models.AlloggiatiService.TrackedRecord;

namespace BookingCalendarApi.Services
{
    public class TrackedRecordSerializer : ITrackedRecordSerializer
    {
        public string Serialize(TrackedRecord record) =>
            $"{SerializeType(record.Type)}" +
            $"{record.Arrival:dd/MM/yyyy}" +
            $"{record.Nights:D2}" +
            $"{record.Surname.PadRight(50, ' ')[..50]}" +
            $"{record.Name.PadRight(30, ' ')[..30]}" +
            $"{SerializeGender(record.Sex)}" +
            $"{record.BirthDate:dd/MM/yyyy}" +
            $"{(record.PlaceOfBirth != null ? record.PlaceOfBirth.ToString() : new string(' ', 9))}" +
            $"{(record.ProvinceOfBirth != null ? record.PlaceOfBirth.ToString() : new string(' ', 2))}" +
            $"{record.StateOfBirth}" +
            $"{record.Citizenship}" +
            $"{(record.DocType != null ? record.DocType.ToString()?.ToUpper() : new string(' ', 5))}" +
            $"{(record.DocNumber != null ? record.DocNumber.PadRight(20, ' ') : new string(' ', 20))}" +
            $"{(record.DocIssuer != null ? record.DocIssuer.ToString() : new string(' ', 9))}" +
            $"\r\n";

        private static string SerializeType(AccomodatedType type) => type switch
        {
            AccomodatedType.SingleGuest => "16",
            AccomodatedType.FamilyHead => "17",
            AccomodatedType.GroupHead => "18",
            AccomodatedType.FamilyMember => "19",
            AccomodatedType.GroupMember => "20",
            _ => throw new NotImplementedException()
        };

        private static string SerializeGender(Gender gender) => gender switch
        {
            Gender.Male => "1",
            Gender.Female => "2",
            _ => throw new NotImplementedException()
        };
    }
}
