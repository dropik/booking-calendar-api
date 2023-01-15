using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using static BookingCalendarApi.Models.AlloggiatiService.TrackedRecord;

namespace BookingCalendarApi.Services
{
    public class TrackedRecordsComposer : ITrackedRecordsComposer
    {
        private readonly INationConverter _nationConverter;
        private readonly IPlaceConverter _placeConverter;

        public TrackedRecordsComposer(INationConverter nationConverter, IPlaceConverter placeConverter)
        {
            _nationConverter = nationConverter;
            _placeConverter = placeConverter;
        }

        public IEnumerable<string> Compose(IEnumerable<AssignedBooking<Guest>> source)
        {
            var trackedRecordsBlocks = source
                .Select(booking => booking.Rooms
                    .SelectMany(
                        room => room.Guests,
                        (room, guest) => new TrackedRecord()
                        {
                            Arrival = DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                            Nights = (ushort)(DateTime.ParseExact(room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(room.Arrival, "yyyyMMdd", null)).Days,
                            Surname = guest.LastName,
                            Name = guest.FirstName,
                            Sex = guest.Gender switch
                            {
                                Guest.Sex.M => Gender.Male,
                                Guest.Sex.F => Gender.Female,
                                _ => Gender.Male,
                            },
                            BirthDate = DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null),
                            PlaceOfBirth = guest.BirthCountry != null && guest.BirthCountry == "IT" && guest.BirthCity != null && guest.BirthCity.Trim() != "" ? _placeConverter.GetPlaceCodeByDescription(guest.BirthCity) : null,
                            ProvinceOfBirth = guest.BirthCounty,
                            StateOfBirth = _nationConverter.GetCodeByIso(guest.BirthCountry ?? "IT"),
                            Citizenship = _nationConverter.GetCodeByIso(guest.Citizenship ?? "IT"),
                            DocType = guest.DocType switch
                            {
                                Guest.DocumentType.ID => DocumentType.Ident,
                                Guest.DocumentType.PP => DocumentType.Pasor,
                                Guest.DocumentType.DL => DocumentType.Paten,
                                _ => null
                            },
                            DocNumber = guest.DocNumber,
                            DocIssuer = guest.DocCity != null
                                ? _placeConverter.GetPlaceCodeByDescription(guest.DocCity) ?? (guest.DocCountry != null ? _nationConverter.GetCodeByIso(guest.DocCountry) : null)
                                : guest.DocCountry != null ? _nationConverter.GetCodeByIso(guest.DocCountry) : null
                        }));

            var recordBlocksWithCorrectPlaceOfBirth = trackedRecordsBlocks
                .Select(trackedRecords => trackedRecords
                    .Where(record => record.StateOfBirth != 100000100 || record.PlaceOfBirth != null && record.ProvinceOfBirth?.Length == 2)
                    .ToList())
                .ToList();

            foreach (var block in recordBlocksWithCorrectPlaceOfBirth)
            {
                foreach (var record in block)
                {
                    GuessType(record, block);
                }
            }

            return recordBlocksWithCorrectPlaceOfBirth
                .Where(block => block
                    .Where(record =>
                        record.Type == AccomodatedType.SingleGuest && block.Count == 1 ||
                        record.Type == AccomodatedType.FamilyHead ||
                        record.Type == AccomodatedType.GroupHead
                     )
                    .Any()
                )
                .SelectMany(
                    block => block.OrderBy(r => r.Type),
                    (block, record) => SerializeRecord(record)
                 );
        }

        private static void GuessType(TrackedRecord recordToSolve, IEnumerable<TrackedRecord> recordsBlock)
        {
            if (recordsBlock.Count() == 1)
            {
                recordToSolve.Type = AccomodatedType.SingleGuest;
            }
            else if (recordsBlock.Where(record => record.Type == AccomodatedType.FamilyHead).Any())
            {
                recordToSolve.Type = AccomodatedType.FamilyMember;
            }
            else if (recordsBlock.Where(record => record.Type == AccomodatedType.GroupHead).Any())
            {
                recordToSolve.Type = AccomodatedType.GroupMember;
            }
            else
            {
                var canBeHead =
                    recordToSolve.DocType != null &&
                    recordToSolve.DocNumber != null &&
                    recordToSolve.DocIssuer != null &&
                    Utils.GetAgeAtArrival(recordToSolve.BirthDate.ToString("yyyyMMdd"), recordToSolve.Arrival.ToString("yyyyMMdd")) >= 18;

                var mightBeFamily =
                    (
                        recordsBlock.Count() == 2 &&
                        recordsBlock.Where(record => record.Sex == Gender.Male).Any() &&
                        recordsBlock.Where(record => record.Sex == Gender.Female).Any()
                    ) ||
                    (
                        recordsBlock.Count() > 2 &&
                        recordsBlock.Count() <= 5 &&
                        recordsBlock.Where(record => Utils.GetAgeAtArrival(record.BirthDate.ToString("yyyyMMdd"), record.Arrival.ToString("yyyyMMdd")) >= 18).Count() < recordsBlock.Count()
                    ) ||
                    recordsBlock.Where(record => record.Surname == recordToSolve.Surname).Count() > 2;

                recordToSolve.Type = canBeHead
                    ? mightBeFamily ? AccomodatedType.FamilyHead : AccomodatedType.GroupHead
                    : mightBeFamily ? AccomodatedType.FamilyMember : AccomodatedType.GroupMember;
            }
        }

        private static string SerializeRecord(TrackedRecord record) =>
            $"{SerializeType(record.Type)}" +
            $"{record.Arrival:dd/MM/yyyy}" +
            $"{record.Nights:D2}" +
            $"{record.Surname.PadRight(50, ' ')[..50]}" +
            $"{record.Name.PadRight(30, ' ')[..30]}" +
            $"{SerializeGender(record.Sex)}" +
            $"{record.BirthDate:dd/MM/yyyy}" +
            $"{(record.PlaceOfBirth != null ? record.PlaceOfBirth.ToString() : new string(' ', 9))}" +
            $"{(record.ProvinceOfBirth != null && record.ProvinceOfBirth.Length == 2 ? record.ProvinceOfBirth : new string(' ', 2))}" +
            $"{record.StateOfBirth}" +
            $"{record.Citizenship}" +
            $"{(record.DocType != null ? record.DocType.ToString()?.ToUpper() : new string(' ', 5))}" +
            $"{(record.DocNumber != null ? record.DocNumber.PadRight(20, ' ') : new string(' ', 20))}" +
            $"{(record.DocIssuer != null ? record.DocIssuer.ToString() : new string(' ', 9))}";

        private static string SerializeType(AccomodatedType type) => type switch
        {
            AccomodatedType.SingleGuest => "16",
            AccomodatedType.FamilyHead => "17",
            AccomodatedType.GroupHead => "18",
            AccomodatedType.FamilyMember => "19",
            AccomodatedType.GroupMember => "20",
            _ => "20",
        };

        private static string SerializeGender(Gender gender) => gender switch
        {
            Gender.Male => "1",
            Gender.Female => "2",
            _ => "1",
        };
    }
}
