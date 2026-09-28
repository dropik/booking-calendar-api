namespace BookingCalendarApi.UnitTests.Models
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("20000615", "20100614", 9)]   // day before the birthday
        [InlineData("20000615", "20100615", 10)]  // on the birthday
        [InlineData("20000615", "20100616", 10)]  // day after the birthday
        [InlineData("20000615", "20100515", 9)]   // month before, same day
        [InlineData("20000615", "20100520", 9)]   // month before, later day
        [InlineData("20000615", "20100701", 10)]  // month after, earlier day
        [InlineData("20001231", "20100101", 9)]   // year boundary
        public void GetAgeAtArrival_counts_completed_years(string birthDate, string arrival, int expectedAge)
        {
            Utils.GetAgeAtArrival(birthDate, arrival).Should().Be(expectedAge);
        }

        [Fact]
        public void GetAgeAtArrival_rejects_dates_not_in_compact_format()
        {
            var act = () => Utils.GetAgeAtArrival("2000-06-15", "20100615");

            act.Should().Throw<FormatException>();
        }
    }
}
