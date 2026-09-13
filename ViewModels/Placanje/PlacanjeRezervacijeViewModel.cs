using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PametniParkingSistem.ViewModels.Placanje
{
    public class PlacanjeRezervacijeViewModel : IValidatableObject
    {
        public double Iznos { get; set; }

        
        [Required(ErrorMessage = "Ime vlasnika kartice je obavezno.")]
        public string ImeVlasnikaKartice { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj kartice je obavezan.")]
        [RegularExpression(@"^(\d{4}\s?){4}$",
            ErrorMessage = "Broj kartice mora sadržavati tačno 16 cifara.")]
        public string BrojKartice { get; set; } = string.Empty;



        [Required(ErrorMessage = "Datum isteka je obavezan.")]
        public string DatumIsteka { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV je obavezan.")]
        [RegularExpression(@"^\d{3}$",
            ErrorMessage = "CVV mora sadržavati tačno 3 cifre.")]
        public string CVV { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var rezultati = new List<ValidationResult>();

            if (!string.IsNullOrWhiteSpace(DatumIsteka))
            {
                try
                {
                    var parts = DatumIsteka.Split('/');

                    if (parts.Length != 2)
                    {
                        rezultati.Add(new ValidationResult(
                            "Unesite datum isteka u formatu MM/YY.",
                            new[] { nameof(DatumIsteka) }));

                        return rezultati;
                    }

                    int mjesec = int.Parse(parts[0]);
                    int godina = int.Parse(parts[1]) + 2000;

                    if (mjesec < 1 || mjesec > 12)
                    {
                        rezultati.Add(new ValidationResult(
                            "Neispravan mjesec isteka kartice.",
                            new[] { nameof(DatumIsteka) }));

                        return rezultati;
                    }

                    var datumIstekaKartice =
                        new DateTime(godina, mjesec,
                        DateTime.DaysInMonth(godina, mjesec));

                    if (datumIstekaKartice < DateTime.Now.Date)
                    {
                        rezultati.Add(new ValidationResult(
                            "Kartica je istekla. Unesite važeći datum isteka.",
                            new[] { nameof(DatumIsteka) }));
                    }
                }
                catch
                {
                    rezultati.Add(new ValidationResult(
                        "Neispravan format datuma isteka.",
                        new[] { nameof(DatumIsteka) }));
                }
            }

            return rezultati;
        }
    }
}