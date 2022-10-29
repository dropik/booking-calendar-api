using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class TrackedRecordsComposer : ITrackedRecordsComposer
    {
        private readonly INationConverter _nationConverter;
        private readonly IPlaceConverter _placeConverter;
        private readonly IAccomodatedTypeSolver _accomodatedTypeSolver;
        private readonly ITrackedRecordSerializer _trackedRecordSerializer;

        public TrackedRecordsComposer(INationConverter nationConverter, IPlaceConverter placeConverter, IAccomodatedTypeSolver accomodatedTypeSolver, ITrackedRecordSerializer trackedRecordSerializer)
        {
            _nationConverter = nationConverter;
            _placeConverter = placeConverter;
            _accomodatedTypeSolver = accomodatedTypeSolver;
            _trackedRecordSerializer = trackedRecordSerializer;
        }

        public IEnumerable<string> Compose(IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> source)
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
                                Models.Iperbooking.Guests.Guest.Sex.M => TrackedRecord.Gender.Male,
                                Models.Iperbooking.Guests.Guest.Sex.F => TrackedRecord.Gender.Female,
                                _ => throw new Exception("Gender was not found")
                            },
                            BirthDate = DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null),
                            PlaceOfBirth = guest.BirthCountry != null && guest.BirthCountry == "IT" && guest.BirthCity != null && guest.BirthCity.Trim() != "" ? _placeConverter.GetPlaceCodeByDescription(guest.BirthCity) : null,
                            ProvinceOfBirth = guest.BirthCounty,
                            StateOfBirth = _nationConverter.GetCodeByIso(guest.BirthCountry ?? "IT"),
                            Citizenship = _nationConverter.GetCodeByIso(guest.Citizenship ?? "IT"),
                            DocType = guest.DocType switch
                            {
                                Models.Iperbooking.Guests.Guest.DocumentType.ID => TrackedRecord.DocumentType.Ident,
                                Models.Iperbooking.Guests.Guest.DocumentType.PP => TrackedRecord.DocumentType.Pasor,
                                Models.Iperbooking.Guests.Guest.DocumentType.DL => TrackedRecord.DocumentType.Paten,
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
                    _accomodatedTypeSolver.Solve(record, block);
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
                    block => block,
                    (block, record) => _trackedRecordSerializer.Serialize(record)
                 );
        }
    }
}
