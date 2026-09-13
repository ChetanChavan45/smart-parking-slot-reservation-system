using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Lozinka { get; set; } = string.Empty;

        [Display(Name = "Zapamti me")]
        public bool ZapamtiMe { get; set; }
    }
}