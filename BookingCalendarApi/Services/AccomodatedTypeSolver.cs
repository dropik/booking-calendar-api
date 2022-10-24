using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class AccomodatedTypeSolver : IAccomodatedTypeSolver
    {
        public void Solve(TrackedRecord recordToSolve, IEnumerable<TrackedRecord> recordsBlock, AssignedBooking<Models.Iperbooking.Guests.Guest> booking)
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
