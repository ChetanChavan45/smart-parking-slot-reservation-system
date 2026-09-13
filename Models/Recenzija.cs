using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.Models
{
    public class Recenzija
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5.")]
        public int Ocjena { get; set; }

        public string Komentar { get; set; } = string.Empty;

        public DateTime Datum { get; set; }

        public bool Obrisan { get; set; }

        public string KorisnikId { get; set; } = string.Empty;

        public Korisnik? Korisnik { get; set; }

        public int RezervacijaId { get; set; }

        public Rezervacija? Rezervacija { get; set; }
    }
}