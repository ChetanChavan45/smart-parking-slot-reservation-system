using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class ParkingMjesto
    {
        public int Id { get; set; }

        public string Oznaka { get; set; } = string.Empty;

        public StatusMjesta Status { get; set; }
        public TipMjesta TipMjesta { get; set; }

        public bool Natkriveno { get; set; }

        public double UdaljenostOdUlaza { get; set; }
        public double CijenaPoSatu { get; set; }

        // FK
        public int ParkingZonaId { get; set; }

        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public ParkingZona? ParkingZona { get; set; }

    }
}