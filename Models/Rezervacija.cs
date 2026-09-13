using PametniParkingSistem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }

        public DateTime DatumKreiranja { get; set; }
        public DateTime VrijemePocetka { get; set; }
        public DateTime VrijemeKraja { get; set; }

        [Required(ErrorMessage = "Registracijske tablice su obavezne.")]
        [StringLength(12, MinimumLength = 5,
    ErrorMessage = "Registracijske tablice moraju imati između 5 i 12 znakova.")]
        public string RegistracijskeTablice { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kontakt telefon je obavezan.")]
        [RegularExpression(@"^\+?\d{6,15}$",
            ErrorMessage = "Unesite ispravan broj telefona.")]
        public string KontaktTelefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Unesite ispravan email.")]
        public string EmailZaObavijest { get; set; } = string.Empty;

        public double UkupnaCijena { get; set; }

        public StatusRezervacije StatusRezervacije { get; set; }

        // FK
        public string KorisnikId { get; set; } = string.Empty;
        public int ParkingMjestoId { get; set; }

        // Navigation properties
        public Korisnik? Korisnik { get; set; }
        public ParkingMjesto? ParkingMjesto { get; set; }

        // Jedna rezervacija može imati jednu recenziju
        public Recenzija? Recenzija { get; set; }
    }
}