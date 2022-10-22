﻿using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class CityTaxPeriodTrimmer : CityTaxCalculatorDecoratorBase
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        public CityTaxPeriodTrimmer(string from, string to, ICityTaxCalculator wrappee) : base(wrappee)
        {
            _from = DateTime.ParseExact(from, "yyyy-MM-dd", null);
            _to = DateTime.ParseExact(to, "yyyy-MM-dd", null);
        }

        public override CityTax Calculate(Stay stay)
        {
            var value = base.Calculate(stay);
            var guests = stay.Guests.Count();
            var arrival = DateTime.ParseExact(stay.Arrival, "yyyyMMdd", null);
            var departure = DateTime.ParseExact(stay.Departure, "yyyyMMdd", null);

            // storing in a signed int to protect from underflow during calculations
            int standard = (int)value.Standard;
            int children = (int)value.Children;

            var daysBeforeFrom = (_from - arrival).Days;
            if (daysBeforeFrom > 0)
            {
                var diff = guests * daysBeforeFrom;
                standard -= diff;
                children -= diff;
            }

            var daysAfterTo = (departure - _to).Days;
            if (daysAfterTo > 0)
            {
                var diff = guests * daysAfterTo;
                standard -= diff;
                children -= diff;
            }

            value.Standard = Convert.ToUInt32(standard >= 0 ? standard : 0);
            value.Children = Convert.ToUInt32(children >= 0 ? children : 0);

            return value;
        }
    }
}
