using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;

namespace BookingCalendarApi.UnitTests.Services
{
    public class NationConverterTests
    {
        private static NationConverter Converter()
        {
            var dataContext = new DataContext();
            dataContext.Nations.Add(new Nation("IT", 100000100, "ITALIA"));
            dataContext.Nations.Add(new Nation("DE", 100000216, "GERMANIA"));
            return new NationConverter(dataContext);
        }

        [Fact]
        public void GetCodeByIso_returns_the_code_of_the_nation()
        {
            Converter().GetCodeByIso("DE").Should().Be(100000216);
        }

        [Fact]
        public void GetCodeByIso_throws_for_unknown_nations()
        {
            var act = () => Converter().GetCodeByIso("XX");

            act.Should().Throw<BookingCalendarException>().WithMessage("Cannot find nation by ISO code").Which.ErrorCode.Should().Be(BCError.MISSING_NATION);
        }
    }

    public class PlaceConverterTests
    {
        private static PlaceConverter Converter(params (long Code, string Description)[] places)
        {
            var dataContext = new DataContext();
            dataContext.Places.AddRange(places.Select(p => new Place { Codice = p.Code, Descrizione = p.Description }));
            return new PlaceConverter(dataContext);
        }

        [Theory]
        [InlineData("pà", 1)]
        [InlineData("pò", 2)]
        [InlineData("pè", 3)]
        [InlineData("pé", 3)]
        [InlineData("pù", 4)]
        [InlineData("pì", 5)]
        [InlineData("px", 6)]
        public void GetPlaceCodeByDescription_normalises_accents_and_case(string description, long expected)
        {
            // unaccented decoys (codes 1x) are just one edit away from the unnormalised input
            var converter = Converter(
                (11, "PA"), (1, "PA'"), (12, "PO"), (2, "PO'"), (13, "PE"), (3, "PE'"),
                (14, "PU"), (4, "PU'"), (15, "PI"), (5, "PI'"), (6, "PX"));

            converter.GetPlaceCodeByDescription(description).Should().Be(expected);
        }

        [Fact]
        public void GetPlaceCodeByDescription_picks_the_closest_description()
        {
            var converter = Converter((1, "ROMA"), (2, "MILANO"), (3, "MILAZZO"));

            converter.GetPlaceCodeByDescription("Milan").Should().Be(2);
        }
    }
}
