using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Placanje
    {
        public int Id { get; set; }

        public string ImeVlasnikaKartice { get; set; } = string.Empty;
        public string BrojKarticeMaskiran { get; set; } = string.Empty;

        public DateTime DatumPlacanja { get; set; }

        public double Iznos { get; set; }

        public StatusPlacanja StatusPlacanja { get; set; }

        public string TransakcijskiBroj { get; set; } = string.Empty;

        // FK
        public int RezervacijaId { get; set; }
        // Navigation property
        public Rezervacija? Rezervacija { get; set; }
    }
}