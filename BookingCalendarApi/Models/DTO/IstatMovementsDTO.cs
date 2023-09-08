﻿using BookingCalendarApi.Models.Clients.C59;

namespace BookingCalendarApi.Models.DTO
{
    public class IstatMovementsDTO
    {
        public string Date { get; set; } = "";
        public long PrevTotal { get; set; }
        public List<MovimentoWSPO> Movements { get; set; } = new();
    }
}
