using C59Service;

namespace BookingCalendarApi.Models
{
    public class MovementsTestResponseItem
    {
        public MovementsTestResponseItem(string date, IEnumerable<movimentoWSPO> movements)
        {
            Date = date;
            Movements = movements;
        }

        public string Date { get; set; }
        public IEnumerable<movimentoWSPO> Movements { get; set; }
    }
}
