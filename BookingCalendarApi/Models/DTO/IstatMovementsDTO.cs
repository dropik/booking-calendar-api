using C59Service;

namespace BookingCalendarApi.Models.DTO
{
    public class IstatMovementsDTO
    {
        public string Date { get; set; } = "";
        public long PrevTotal { get; set; }
        public List<movimentoWSPO> Movements { get; set; } = new();
    }
}
