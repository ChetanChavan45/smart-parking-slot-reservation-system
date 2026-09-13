using System.ComponentModel.DataAnnotations;
using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class PodrskaZahtjev
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naslov je obavezan.")]
        [StringLength(100, ErrorMessage = "Naslov može imati najviše 100 karaktera.")]
        public string Naslov { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis problema je obavezan.")]
        [StringLength(1000, ErrorMessage = "Opis može imati najviše 1000 karaktera.")]
        public string Opis { get; set; } = string.Empty;

        public KategorijaPodrske Kategorija { get; set; }

        public PrioritetPodrske Prioritet { get; set; }

        public StatusPodrske Status { get; set; } = StatusPodrske.Otvoren;

        public DateTime DatumKreiranja { get; set; } = DateTime.Now;

        public DateTime? DatumOdgovora { get; set; }

        [StringLength(1000, ErrorMessage = "Odgovor može imati najviše 1000 karaktera.")]
        public string? Odgovor { get; set; }

        public string KorisnikId { get; set; } = string.Empty;

        public Korisnik? Korisnik { get; set; }
    }
}