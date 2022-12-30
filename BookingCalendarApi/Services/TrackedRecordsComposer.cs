using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public class TrackedRecordsComposer : ITrackedRecordsComposer
    {
        private readonly INationConverter _nationConverter;
        private readonly IPlaceConverter _placeConverter;
        private readonly ITrackedRecordSerializer _trackedRecordSerializer;

        public TrackedRecordsComposer(
            INationConverter nationConverter,
            IPlaceConverter placeConverter,
            ITrackedRecordSerializer trackedRecordSerializer)
        {
            _nationConverter = nationConverter;
            _placeConverter = placeConverter;
            _trackedRecordSerializer = trackedRecordSerializer;
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
                                Guest.Sex.M => TrackedRecord.Gender.Male,
                                Guest.Sex.F => TrackedRecord.Gender.Female,
                                _ => throw new Exception("Gender was not found")
                            },
                            BirthDate = DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null),
                            PlaceOfBirth = guest.BirthCountry != null && guest.BirthCountry == "IT" && guest.BirthCity != null && guest.BirthCity.Trim() != "" ? _placeConverter.GetPlaceCodeByDescription(guest.BirthCity) : null,
                            ProvinceOfBirth = guest.BirthCounty,
                            StateOfBirth = _nationConverter.GetCodeByIso(guest.BirthCountry ?? "IT"),
                            Citizenship = _nationConverter.GetCodeByIso(guest.Citizenship ?? "IT"),
                            DocType = guest.DocType switch
                            {
                                Guest.DocumentType.ID => TrackedRecord.DocumentType.Ident,
                                Guest.DocumentType.PP => TrackedRecord.DocumentType.Pasor,
                                Guest.DocumentType.DL => TrackedRecord.DocumentType.Paten,
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
                        record.Type == TrackedRecord.AccomodatedType.SingleGuest && block.Count == 1 ||
                        record.Type == TrackedRecord.AccomodatedType.FamilyHead ||
                        record.Type == TrackedRecord.AccomodatedType.GroupHead
                     )
                    .Any()
                )
                .SelectMany(
                    block => block.OrderBy(r => r.Type),
                    (block, record) => _trackedRecordSerializer.Serialize(record)
                 );
        }

        private static void GuessType(TrackedRecord recordToSolve, IEnumerable<TrackedRecord> recordsBlock)
        {
            if (recordsBlock.Count() == 1)
            {
                recordToSolve.Type = TrackedRecord.AccomodatedType.SingleGuest;
            }
            else if (recordsBlock.Where(record => record.Type == TrackedRecord.AccomodatedType.FamilyHead).Any())
            {
                recordToSolve.Type = TrackedRecord.AccomodatedType.FamilyMember;
            }
            else if (recordsBlock.Where(record => record.Type == TrackedRecord.AccomodatedType.GroupHead).Any())
            {
                recordToSolve.Type = TrackedRecord.AccomodatedType.GroupMember;
            }
            else
            {
                var canBeHead =
                    recordToSolve.DocType != null &&
                    recordToSolve.DocNumber != null &&
                    recordToSolve.DocIssuer != null &&
                    CityTaxGuestRegistriesFilter.GetAgeAtArrival(recordToSolve.BirthDate.ToString("yyyyMMdd"), recordToSolve.Arrival.ToString("yyyyMMdd")) >= 18;

                var mightBeFamily =
                    (
                        recordsBlock.Count() == 2 &&
                        recordsBlock.Where(record => record.Sex == TrackedRecord.Gender.Male).Any() &&
                        recordsBlock.Where(record => record.Sex == TrackedRecord.Gender.Female).Any()
                    ) ||
                    (
                        recordsBlock.Count() > 2 &&
                        recordsBlock.Count() <= 5 &&
                        recordsBlock.Where(record => CityTaxGuestRegistriesFilter.GetAgeAtArrival(record.BirthDate.ToString("yyyyMMdd"), record.Arrival.ToString("yyyyMMdd")) >= 18).Count() < recordsBlock.Count()
                    ) ||
                    recordsBlock.Where(record => record.Surname == recordToSolve.Surname).Count() > 2;

                recordToSolve.Type = canBeHead
                    ? mightBeFamily ? TrackedRecord.AccomodatedType.FamilyHead : TrackedRecord.AccomodatedType.GroupHead
                    : mightBeFamily ? TrackedRecord.AccomodatedType.FamilyMember : TrackedRecord.AccomodatedType.GroupMember;
            }
        }
    }
}
