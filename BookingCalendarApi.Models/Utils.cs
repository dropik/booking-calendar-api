using System;

namespace BookingCalendarApi
{
    public static class Utils
    {
        public static int GetAgeAtArrival(string birthDate, string arrival)
        {
            var birthDateObj = DateTime.ParseExact(birthDate, "yyyyMMdd", null);
            var arrivalObj = DateTime.ParseExact(arrival, "yyyyMMdd", null);

            int years = arrivalObj.Year - birthDateObj.Year;

            if (
                (
                    birthDateObj.Month == arrivalObj.Month &&
                    arrivalObj.Day < birthDateObj.Day
                ) ||
                arrivalObj.Month < birthDateObj.Month
            )
            {
                years--;
            }

            return years;
        }
    }
}
