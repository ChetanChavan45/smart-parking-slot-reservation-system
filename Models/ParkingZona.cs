namespace PametniParkingSistem.Models
{
    public class ParkingZona
    {
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;
        public string Lokacija { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;

        public double ProsjecnaOcjena { get; set; }

        public ICollection<ParkingMjesto> ParkingMjesta { get; set; }
    = new List<ParkingMjesto>();
    }
}