using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IKorisnikRepository _repository;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public KorisnikService(
            IKorisnikRepository repository,
            IRezervacijaService rezervacijaService,
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _rezervacijaService = rezervacijaService;
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public Task<List<Korisnik>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public async Task<List<Korisnik>> GetVisibleUsersAsync(bool isAdministrator)
        {
            var korisnici = await _repository.GetAllAsync();
            return isAdministrator
                ? korisnici
                : korisnici.Where(k => k.Uloga != Uloga.Administrator).ToList();
        }

        public async Task<Korisnik?> GetAccessibleByIdAsync(string id, bool isAdministrator)
        {
            var korisnik = await _repository.GetByIdAsync(id);
            if (!isAdministrator && korisnik?.Uloga == Uloga.Administrator)
                return null;

            return korisnik;
        }

        public Task<Korisnik?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

        public async Task<int> GetReservationCountAsync(string userId)
        {
            var rezervacije = await _rezervacijaService.GetByKorisnikIdAsync(userId);
            return rezervacije.Count;
        }

        public async Task<KorisnikServiceResult> CreateAsync(Korisnik korisnik)
        {
            korisnik.Ime = korisnik.Ime.Trim();
            korisnik.Prezime = korisnik.Prezime.Trim();
            korisnik.Email = korisnik.Email!.Trim();
            korisnik.UserName = korisnik.Email;
            korisnik.DatumRegistracije = DateTime.Now;

            await _repository.AddAsync(korisnik);
            return KorisnikServiceResult.Success(korisnik);
        }

        public async Task<KorisnikServiceResult> UpdateAccountAsync(
            string id,
            StatusNaloga statusNaloga,
            Uloga? uloga,
            bool isAdministrator)
        {
            var korisnik = await _repository.GetByIdAsync(id);
            if (korisnik == null) return KorisnikServiceResult.Missing();
            if (!isAdministrator && korisnik.Uloga == Uloga.Administrator)
                return KorisnikServiceResult.Denied();

            korisnik.StatusNaloga = statusNaloga;

            if (isAdministrator && uloga.HasValue && korisnik.Uloga != uloga.Value)
            {
                var trenutneRole = await _userManager.GetRolesAsync(korisnik);
                if (trenutneRole.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(korisnik, trenutneRole);
                    if (!removeResult.Succeeded)
                        return IdentityFailure(removeResult.Errors);
                }

                var addResult = await _userManager.AddToRoleAsync(korisnik, uloga.Value.ToString());
                if (!addResult.Succeeded)
                    return IdentityFailure(addResult.Errors);

                korisnik.Uloga = uloga.Value;
            }

            await _repository.UpdateAsync(korisnik);
            return KorisnikServiceResult.Success(korisnik);
        }

        public async Task<KorisnikServiceResult> UpdateProfileAsync(
            string userId,
            Korisnik model,
            IFormFile? profilnaSlika)
        {
            var korisnik = await _repository.GetByIdAsync(userId);
            if (korisnik == null) return KorisnikServiceResult.Missing();

            korisnik.Ime = model.Ime.Trim();
            korisnik.Prezime = model.Prezime.Trim();
            korisnik.Email = model.Email!.Trim();
            korisnik.UserName = korisnik.Email;
            korisnik.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber)
                ? null
                : model.PhoneNumber.Trim();

            if (profilnaSlika is { Length: > 0 })
            {
                var ekstenzija = Path.GetExtension(profilnaSlika.FileName).ToLowerInvariant();
                var dozvoljeneEkstenzije = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!dozvoljeneEkstenzije.Contains(ekstenzija))
                    return KorisnikServiceResult.Failure("Dozvoljeni formati slike su JPG, JPEG, PNG i WEBP.");

                const long maksimalnaVelicina = 5 * 1024 * 1024;
                if (profilnaSlika.Length > maksimalnaVelicina)
                    return KorisnikServiceResult.Failure("Profilna slika može imati najviše 5 MB.");

                var folder = Path.Combine(_environment.WebRootPath, "uploads", "profili");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ekstenzija}";
                var filePath = Path.Combine(folder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await profilnaSlika.CopyToAsync(stream);
                korisnik.ProfilnaSlikaUrl = $"/uploads/profili/{fileName}";
            }

            var result = await _userManager.UpdateAsync(korisnik);
            if (!result.Succeeded) return IdentityFailure(result.Errors);

            await _signInManager.RefreshSignInAsync(korisnik);
            return KorisnikServiceResult.Success(korisnik);
        }

        public async Task<KorisnikServiceResult> ChangePasswordAsync(
            string userId,
            string trenutnaLozinka,
            string novaLozinka)
        {
            var korisnik = await _repository.GetByIdAsync(userId);
            if (korisnik == null) return KorisnikServiceResult.Missing();

            var result = await _userManager.ChangePasswordAsync(korisnik, trenutnaLozinka, novaLozinka);
            if (!result.Succeeded) return IdentityFailure(result.Errors);

            await _signInManager.RefreshSignInAsync(korisnik);
            return KorisnikServiceResult.Success(korisnik);
        }

        public async Task<KorisnikServiceResult> SetStatusAsync(
            string id,
            StatusNaloga status,
            bool isAdministrator)
        {
            var korisnik = await _repository.GetByIdAsync(id);
            if (korisnik == null) return KorisnikServiceResult.Missing();
            if (!isAdministrator && korisnik.Uloga == Uloga.Administrator)
                return KorisnikServiceResult.Denied();

            korisnik.StatusNaloga = status;
            await _repository.UpdateAsync(korisnik);
            return KorisnikServiceResult.Success(korisnik);
        }

        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);

        private static KorisnikServiceResult IdentityFailure(IEnumerable<IdentityError> errors) =>
            KorisnikServiceResult.Failure(errors.Select(PrevediIdentityGresku).ToArray());

        private static string PrevediIdentityGresku(IdentityError error) => error.Code switch
        {
            "PasswordMismatch" => "Trenutna lozinka nije ispravna.",
            "InvalidUserName" => "Korisničko ime nije ispravno.",
            "InvalidEmail" => "Email adresa nije ispravna.",
            "DuplicateUserName" or "DuplicateEmail" => "Korisnik sa ovom email adresom već postoji.",
            "PasswordTooShort" => "Lozinka mora imati najmanje 6 karaktera.",
            "PasswordRequiresNonAlphanumeric" => "Lozinka mora sadržavati barem jedan specijalni znak.",
            "PasswordRequiresDigit" => "Lozinka mora sadržavati barem jedan broj.",
            "PasswordRequiresLower" => "Lozinka mora sadržavati barem jedno malo slovo.",
            "PasswordRequiresUpper" => "Lozinka mora sadržavati barem jedno veliko slovo.",
            _ => "Došlo je do greške. Provjerite unesene podatke i pokušajte ponovo."
        };
    }
}
