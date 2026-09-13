using Microsoft.AspNetCore.Identity;
using PametniParkingSistem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.Models
{
    public class Korisnik : IdentityUser
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [Display(Name = "Ime")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; } = string.Empty;

        [Display(Name = "Datum registracije")]
        public DateTime DatumRegistracije { get; set; } = DateTime.Now;

        [Display(Name = "Status naloga")]
        public StatusNaloga StatusNaloga { get; set; }

        [Display(Name = "Uloga")]
        public Uloga Uloga { get; set; }

        [Display(Name = "Profilna slika")]
        public string? ProfilnaSlikaUrl { get; set; }

        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
    }
}