using System;

namespace BookingCalendarApi.Models.Clients.C59
{
    public class InviaC59FullRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public long Struttura { get; set; }
        public C59WSPO C59 { get; set; }
    }

    public class C59WSPO
    {
        public DateTime DataMovimentazione { get; set; }
        public bool DataMovimentazioneSpecified { get; set; }
        public bool EsercizioAperto { get; set; }
        public MovimentoWSPO[] Movimenti { get; set; }
        public long TotaleArrivi { get; set; }
        public long TotalePartenze { get; set; }
        public long TotalePresenti { get; set; }
        public long UnitaAbitativeDisponibili { get; set; }
        public long UnitaAbitativeOccupate { get; set; }
    }

    public class MovimentoWSPO
    {
        public long Arrivi { get; set; }
        public bool Italia { get; set; }
        public long Partenze { get; set; }
        public string Targa { get; set; }
    }
}
