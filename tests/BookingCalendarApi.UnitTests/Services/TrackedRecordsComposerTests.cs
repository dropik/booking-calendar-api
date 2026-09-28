using BookingCalendarApi.Models.Iperbooking.Bookings;
using GuestModel = BookingCalendarApi.Models.Iperbooking.Guests.Guest;
using BookingCalendarApi.Services;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class TrackedRecordsComposerTests
    {
        private const long Italy = 100000100;
        private const long Germany = 100000216;
        private const long Rome = 412058091;
        private const long Milan = 403015146;

        private readonly INationConverter _nations = Substitute.For<INationConverter>();
        private readonly IPlaceConverter _places = Substitute.For<IPlaceConverter>();
        private readonly TrackedRecordsComposer _composer;

        public TrackedRecordsComposerTests()
        {
            _nations.GetCodeByIso("IT").Returns(Italy);
            _nations.GetCodeByIso("DE").Returns(Germany);
            _places.GetPlaceCodeByDescription("Roma").Returns(Rome);
            _places.GetPlaceCodeByDescription("Milano").Returns(Milan);
            _places.GetPlaceCodeByDescription("Berlin").Returns((long?)null);
            _composer = new TrackedRecordsComposer(_nations, _places);
        }

        private static GuestModel Italian(string lastName = "Rossi", string firstName = "Mario", GuestModel.Sex sex = GuestModel.Sex.M, string birthDate = "19800115")
        {
            var guest = Guest("1", 1, firstName, lastName, birthDate);
            guest.Gender = sex;
            guest.BirthCountry = "IT";
            guest.BirthCity = "Roma";
            guest.BirthCounty = "RM";
            guest.Citizenship = "IT";
            guest.DocType = GuestModel.DocumentType.ID;
            guest.DocNumber = "AB123";
            guest.DocCity = "Roma";
            guest.DocCountry = "IT";
            return guest;
        }

        private static GuestModel German(string lastName = "Muller", string firstName = "Anna", GuestModel.Sex sex = GuestModel.Sex.F, string birthDate = "19900301")
        {
            var guest = Guest("2", 1, firstName, lastName, birthDate);
            guest.Gender = sex;
            guest.BirthCountry = "DE";
            guest.Citizenship = "DE";
            guest.DocType = GuestModel.DocumentType.PP;
            guest.DocNumber = "P9";
            guest.DocCountry = "DE";
            return guest;
        }

        private static GuestModel Minor(string lastName = "Rossi", string firstName = "Luca")
        {
            var guest = Italian(lastName, firstName, birthDate: "20150101");
            guest.DocNumber = null;
            return guest;
        }

        private static AssignedBooking<GuestModel> Block(string arrival, string departure, params GuestModel[] guests)
        {
            return new AssignedBooking<GuestModel>(Booking(1))
            {
                Rooms =
                {
                    new AssignedRoom<GuestModel>(1, "Doppia", arrival, departure, "R1") { RoomId = 1, Guests = guests.ToList() },
                },
            };
        }

        private List<string> Compose(params AssignedBooking<GuestModel>[] blocks) => _composer.Compose(blocks).ToList();

        private static string Expected(
            string type, string arrival, string nights, string surname, string name, string sex, string birthDate,
            string place, string province, long state, long citizenship, string docType, string docNumber, string docIssuer)
        {
            return type + arrival + nights + surname.PadRight(50) + name.PadRight(30) + sex + birthDate
                + place + province + state + citizenship + docType + docNumber.PadRight(20) + docIssuer;
        }

        private static string TypeOf(string record) => record.Substring(0, 2);

        private static string SurnameOf(string record) => record.Substring(14, 50).Trim();

        [Fact]
        public void Single_italian_guest_is_serialized_as_fixed_width_record()
        {
            var records = Compose(Block("20230610", "20230613", Italian()));

            records.Should().Equal(Expected(
                "16", "10/06/2023", "03", "ROSSI", "MARIO", "1", "15/01/1980",
                Rome.ToString(), "RM", Italy, Italy, "IDENT", "AB123", Rome.ToString()));
            records[0].Should().HaveLength(168);
        }

        [Fact]
        public void Foreign_guest_has_blank_place_and_province_and_document_issued_by_country()
        {
            var records = Compose(Block("20230610", "20230611", German()));

            records.Should().Equal(Expected(
                "16", "10/06/2023", "01", "MULLER", "ANNA", "2", "01/03/1990",
                new string(' ', 9), "  ", Germany, Germany, "PASOR", "P9", Germany.ToString()));
        }

        [Fact]
        public void Foreign_guest_born_in_a_known_italian_city_name_still_has_blank_place()
        {
            var guest = German();
            guest.BirthCity = "Roma";

            Compose(Block("20230610", "20230611", guest)).Single().Substring(105, 9).Should().Be(new string(' ', 9));
        }

        [Fact]
        public void Document_city_is_not_looked_up_when_missing()
        {
            Compose(Block("20230610", "20230611", German()));

            _places.DidNotReceive().GetPlaceCodeByDescription(null!);
        }

        [Fact]
        public void Document_city_that_is_not_a_known_place_falls_back_to_document_country()
        {
            var guest = German();
            guest.DocCity = "Berlin";

            Compose(Block("20230610", "20230611", guest)).Single().Should().EndWith(Germany.ToString());
        }

        [Fact]
        public void Document_city_takes_precedence_over_document_country()
        {
            var guest = German();
            guest.DocCity = "Milano";

            Compose(Block("20230610", "20230611", guest)).Single().Should().EndWith(Milan.ToString());
        }

        [Fact]
        public void Missing_document_data_is_left_blank()
        {
            var guest = German();
            guest.DocNumber = null;
            guest.DocCountry = null;

            Compose(Block("20230610", "20230611", guest)).Single()
                .Should().EndWith("PASOR" + new string(' ', 20) + new string(' ', 9));
        }

        [Fact]
        public void Document_city_without_known_place_and_without_country_is_left_blank()
        {
            var guest = German();
            guest.DocCity = "Berlin";
            guest.DocCountry = null;

            Compose(Block("20230610", "20230611", guest)).Single().Should().EndWith(new string(' ', 9));
        }

        [Theory]
        [InlineData(GuestModel.DocumentType.ID, "IDENT")]
        [InlineData(GuestModel.DocumentType.PP, "PASOR")]
        [InlineData(GuestModel.DocumentType.DL, "PATEN")]
        [InlineData(GuestModel.DocumentType.XX, "IDELE")]
        public void Document_types_are_mapped_to_police_codes(GuestModel.DocumentType documentType, string expected)
        {
            var guest = Italian();
            guest.DocType = documentType;

            Compose(Block("20230610", "20230611", guest)).Single().Substring(134, 5).Should().Be(expected);
        }

        [Fact]
        public void Missing_citizenship_defaults_to_italy()
        {
            var guest = German();
            guest.Citizenship = null;

            Compose(Block("20230610", "20230611", guest)).Single().Substring(116, 18).Should().Be($"{Germany}{Italy}");
        }

        [Fact]
        public void Missing_birth_country_means_italy_without_place_of_birth_so_the_guest_is_dropped()
        {
            var guest = Italian();
            guest.BirthCountry = null;
            guest.DocCity = null;

            Compose(Block("20230610", "20230611", guest)).Should().BeEmpty();
            _nations.Received().GetCodeByIso("IT");
            _places.DidNotReceive().GetPlaceCodeByDescription("Roma");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Italian_guest_without_birth_city_is_dropped(string? birthCity)
        {
            var guest = Italian();
            guest.BirthCity = birthCity;

            Compose(Block("20230610", "20230611", guest)).Should().BeEmpty();
            _places.DidNotReceive().GetPlaceCodeByDescription(Arg.Is<string>(s => s == null || s.Trim() == ""));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("R")]
        [InlineData("RMA")]
        public void Italian_guest_without_two_letters_province_is_dropped(string? province)
        {
            var guest = Italian();
            guest.BirthCounty = province;

            Compose(Block("20230610", "20230611", guest)).Should().BeEmpty();
        }

        [Fact]
        public void Foreign_guest_with_province_keeps_it_only_when_two_letters_long()
        {
            var twoLetters = German();
            twoLetters.BirthCounty = "BY";
            var longer = German();
            longer.BirthCounty = "BAY";

            Compose(Block("20230610", "20230611", twoLetters)).Single().Substring(114, 2).Should().Be("BY");
            Compose(Block("20230610", "20230611", longer)).Single().Substring(114, 2).Should().Be("  ");
        }

        [Fact]
        public void Long_names_are_truncated_to_their_field_width()
        {
            var guest = Italian(lastName: new string('S', 60), firstName: new string('N', 40));

            var record = Compose(Block("20230610", "20230611", guest)).Single();

            record.Substring(14, 50).Should().Be(new string('S', 50));
            record.Substring(64, 30).Should().Be(new string('N', 30));
            record.Should().HaveLength(168);
        }

        [Fact]
        public void Adult_man_and_woman_are_a_family_with_the_first_documented_adult_as_head()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), German(lastName: "Bianchi")));

            records.Select(TypeOf).Should().Equal("17", "19");
            records.Select(SurnameOf).Should().Equal("ROSSI", "BIANCHI");
        }

        [Fact]
        public void Two_adult_men_are_a_group()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), Italian("Verdi", "Luigi")));

            records.Select(TypeOf).Should().Equal("18", "20");
        }

        [Fact]
        public void Two_adult_women_are_a_group()
        {
            var records = Compose(Block("20230610", "20230611", German(), German("Schmidt", "Eva")));

            records.Select(TypeOf).Should().Equal("18", "20");
        }

        [Fact]
        public void Two_adults_sharing_the_surname_are_not_enough_for_a_family()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), Italian(firstName: "Luigi")));

            records.Select(TypeOf).Should().Equal("18", "20");
        }

        [Fact]
        public void Three_adults_of_mixed_sex_with_different_surnames_are_a_group()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), German(lastName: "Bianchi"), Italian("Verdi", "Luigi")));

            records.Select(TypeOf).Should().Equal("18", "20", "20");
        }

        [Fact]
        public void Head_is_listed_first_even_when_found_later()
        {
            var records = Compose(Block("20230610", "20230611", Minor(), Italian(), German(lastName: "Rossi")));

            records.Select(TypeOf).Should().Equal("17", "19", "19");
            records.Select(r => r.Substring(64, 30).Trim()).Should().Equal("MARIO", "LUCA", "ANNA");
        }

        [Fact]
        public void Three_to_five_guests_with_a_minor_are_a_family()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), Italian("Verdi", "Luigi"), Minor("Neri")));

            records.Select(TypeOf).Should().Equal("17", "19", "19");
        }

        [Fact]
        public void Five_guests_with_a_minor_are_a_family()
        {
            var records = Compose(Block("20230610", "20230611", Italian("A"), Italian("B"), Italian("C"), Italian("D"), Minor("E")));

            records.Select(TypeOf).Should().Equal("17", "19", "19", "19", "19");
        }

        [Fact]
        public void Guest_turning_eighteen_on_arrival_counts_as_adult_when_looking_for_families()
        {
            var records = Compose(Block("20230610", "20230611", Italian("A"), Italian("B"), Italian("C", birthDate: "20050610")));

            records.Select(TypeOf).Should().Equal("18", "20", "20");
        }

        [Fact]
        public void Three_adults_with_different_surnames_are_a_group()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), Italian("Verdi", "Luigi"), Italian("Neri", "Paolo")));

            records.Select(TypeOf).Should().Equal("18", "20", "20");
        }

        [Fact]
        public void More_than_two_adults_sharing_the_surname_are_a_family()
        {
            var records = Compose(Block("20230610", "20230611", Italian(), Italian(firstName: "Luigi"), Italian(firstName: "Paolo")));

            records.Select(TypeOf).Should().Equal("17", "19", "19");
        }

        [Fact]
        public void Six_guests_with_a_minor_and_different_surnames_are_a_group()
        {
            var records = Compose(Block("20230610", "20230611",
                Italian("A"), Italian("B"), Italian("C"), Italian("D"), Italian("E"), Minor("F")));

            records.Select(TypeOf).Should().Equal("18", "20", "20", "20", "20", "20");
        }

        [Fact]
        public void Guest_turning_eighteen_on_arrival_can_be_head()
        {
            var records = Compose(Block("20230610", "20230611", Italian(birthDate: "20050610"), Italian("Verdi", "Luigi")));

            records.Select(TypeOf).Should().Equal("18", "20");
            records.Select(SurnameOf).Should().Equal("ROSSI", "VERDI");
        }

        [Fact]
        public void Guest_turning_eighteen_the_day_after_arrival_cannot_be_head()
        {
            var records = Compose(Block("20230610", "20230611", Italian(birthDate: "20050611"), Italian("Verdi", "Luigi")));

            records.Select(TypeOf).Should().Equal("18", "20");
            records.Select(SurnameOf).Should().Equal("VERDI", "ROSSI");
        }

        [Fact]
        public void Guest_without_document_number_cannot_be_head()
        {
            var withoutDocument = Italian();
            withoutDocument.DocNumber = null;

            var records = Compose(Block("20230610", "20230611", withoutDocument, Italian("Verdi", "Luigi")));

            records.Select(SurnameOf).Should().Equal("VERDI", "ROSSI");
        }

        [Fact]
        public void Guest_without_document_issuer_cannot_be_head()
        {
            var withoutIssuer = Italian();
            withoutIssuer.DocCity = null;
            withoutIssuer.DocCountry = null;

            var records = Compose(Block("20230610", "20230611", withoutIssuer, Italian("Verdi", "Luigi")));

            records.Select(SurnameOf).Should().Equal("VERDI", "ROSSI");
        }

        [Fact]
        public void Blocks_without_a_head_are_dropped()
        {
            var records = Compose(
                Block("20230610", "20230611", Minor(), Minor(firstName: "Anna")),
                Block("20230610", "20230611", Italian("Verdi")));

            records.Select(SurnameOf).Should().Equal("VERDI");
        }

        [Fact]
        public void Guests_of_all_rooms_of_a_booking_form_one_block()
        {
            var booking = new AssignedBooking<GuestModel>(Booking(1))
            {
                Rooms =
                {
                    new AssignedRoom<GuestModel>(1, "Doppia", "20230610", "20230611", "R1") { Guests = { Italian() } },
                    new AssignedRoom<GuestModel>(2, "Doppia", "20230610", "20230611", "R1") { Guests = { Italian("Verdi", "Luigi") } },
                },
            };

            Compose(booking).Select(TypeOf).Should().Equal("18", "20");
        }

        [Fact]
        public void Every_booking_is_a_separate_block()
        {
            var records = Compose(Block("20230610", "20230611", Italian()), Block("20230610", "20230611", German()));

            records.Select(TypeOf).Should().Equal("16", "16");
        }

        [Fact]
        public void Single_guest_left_after_dropping_invalid_guests_is_single()
        {
            var invalid = Italian("Verdi");
            invalid.BirthCity = null;

            Compose(Block("20230610", "20230611", Italian(), invalid)).Select(TypeOf).Should().Equal("16");
        }
    }
}
