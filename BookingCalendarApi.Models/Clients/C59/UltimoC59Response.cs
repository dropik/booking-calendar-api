using System;

namespace BookingCalendarApi.Models.Clients.C59
{
    public class UltimoC59Response
    {
        public MovimentiC59 @Return { get; set; }
    }

    public class MovimentiC59
    {
        public C59PO[] ElencoC59 { get; set; }
    }

    public class C59PO
    {
        public DateTime DataMovimentazione { get; set; }
        public long TotalePresenti { get; set; }
    }
}
