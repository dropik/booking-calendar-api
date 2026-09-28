using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Exceptions;

using static BookingCalendarApi.Models.AlloggiatiService.TrackedRecord;

namespace BookingCalendarApi.UnitTests.Models
{
    public class TrackedRecordTests
    {
        [Fact]
        public void Names_are_upper_cased_and_accents_replaced_with_apostrophes()
        {
            var record = new TrackedRecord { Surname = "Forlì-Cantù", Name = "Nicolò Andrèa José Mà" };

            record.Surname.Should().Be("FORLI'-CANTU'");
            record.Name.Should().Be("NICOLO' ANDRE'A JOSE' MA'");
        }

        [Fact]
        public void Nights_up_to_thirty_are_accepted()
        {
            new TrackedRecord { Nights = 30 }.Nights.Should().Be(30);
        }

        [Fact]
        public void Nights_over_thirty_are_rejected()
        {
            var act = () => new TrackedRecord { Nights = 31 };

            act.Should().Throw<BookingCalendarException>().Which.ErrorCode.Should().Be(BCError.MAX_STAY_EXCEEDED);
        }

        [Theory]
        [InlineData(AccomodatedType.SingleGuest)]
        [InlineData(AccomodatedType.FamilyHead)]
        [InlineData(AccomodatedType.GroupHead)]
        public void Document_is_kept_for_single_guests_and_heads(AccomodatedType type)
        {
            var record = new TrackedRecord { Type = type, DocType = DocumentType.Pasor, DocNumber = "X1", DocIssuer = 5 };

            record.DocType.Should().Be(DocumentType.Pasor);
            record.DocNumber.Should().Be("X1");
            record.DocIssuer.Should().Be(5);
        }

        [Theory]
        [InlineData(AccomodatedType.FamilyMember)]
        [InlineData(AccomodatedType.GroupMember)]
        public void Document_is_dropped_for_members_when_type_is_known_first(AccomodatedType type)
        {
            var record = new TrackedRecord { Type = type, DocType = DocumentType.Pasor, DocNumber = "X1", DocIssuer = 5 };

            record.DocType.Should().BeNull();
            record.DocNumber.Should().BeNull();
            record.DocIssuer.Should().BeNull();
        }
    }
}
