using Microsoft.AspNetCore.Identity;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.ViewModels.Account;

namespace PametniParkingSistem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;

        public AccountService(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AccountServiceResult> RegisterAsync(
            RegisterViewModel model)
        {
            var postojeciKorisnik =
                await _userManager.FindByEmailAsync(model.Email);

            if (postojeciKorisnik != null)
            {
                return AccountServiceResult.Failure(
                    "Korisnik sa ovom email adresom već postoji."
                );
            }

            var korisnik = new Korisnik
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true,
                DatumRegistracije = DateTime.Now,
                StatusNaloga = StatusNaloga.Aktivan,
                Uloga = Uloga.RegistrovaniKorisnik
            };

            var createResult =
                await _userManager.CreateAsync(korisnik, model.Lozinka);

            if (!createResult.Succeeded)
            {
                return AccountServiceResult.Failure(
                    createResult.Errors.Select(PrevediIdentityGresku)
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(
                korisnik,
                Uloga.RegistrovaniKorisnik.ToString()
            );

            if (!roleResult.Succeeded)
            {
                // Ako dodavanje uloge nije uspjelo, uklanjamo upravo
                // kreiranog korisnika da ne ostane nepotpun nalog.
                await _userManager.DeleteAsync(korisnik);

                return AccountServiceResult.Failure(
                    roleResult.Errors.Select(PrevediIdentityGresku)
                );
            }

            await _signInManager.SignInAsync(
                korisnik,
                isPersistent: false
            );

            return AccountServiceResult.Success(korisnik);
        }

        public async Task<AccountServiceResult> LoginAsync(
            LoginViewModel model)
        {
            var korisnik =
                await _userManager.FindByEmailAsync(model.Email);

            if (korisnik == null)
            {
                return AccountServiceResult.Failure(
                    "Neispravan email ili lozinka."
                );
            }

            if (korisnik.StatusNaloga != StatusNaloga.Aktivan)
            {
                return AccountServiceResult.Failure(
                    "Korisnički nalog nije aktivan."
                );
            }

            var signInResult =
                await _signInManager.PasswordSignInAsync(
                    korisnik,
                    model.Lozinka,
                    model.ZapamtiMe,
                    lockoutOnFailure: false
                );

            if (signInResult.Succeeded)
            {
                return AccountServiceResult.Success(korisnik);
            }

            if (signInResult.IsLockedOut)
            {
                return AccountServiceResult.Failure(
                    "Korisnički nalog je privremeno zaključan."
                );
            }

            if (signInResult.IsNotAllowed)
            {
                return AccountServiceResult.Failure(
                    "Prijava trenutno nije dozvoljena za ovaj nalog."
                );
            }

            if (signInResult.RequiresTwoFactor)
            {
                return AccountServiceResult.Failure(
                    "Za ovaj nalog je potrebna dodatna potvrda prijave."
                );
            }

            return AccountServiceResult.Failure(
                "Neispravan email ili lozinka."
            );
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        private static string PrevediIdentityGresku(
            IdentityError error)
        {
            return error.Code switch
            {
                "DefaultError" =>
                    "Došlo je do greške. Pokušajte ponovo.",

                "ConcurrencyFailure" =>
                    "Došlo je do konflikta pri spremanju podataka. Pokušajte ponovo.",

                "PasswordMismatch" =>
                    "Lozinka nije ispravna.",

                "InvalidToken" =>
                    "Token nije ispravan.",

                "LoginAlreadyAssociated" =>
                    "Ova prijava je već povezana sa drugim korisnikom.",

                "InvalidUserName" =>
                    "Korisničko ime nije ispravno.",

                "InvalidEmail" =>
                    "Email adresa nije ispravna.",

                "DuplicateUserName" =>
                    "Korisnik sa ovom email adresom već postoji.",

                "DuplicateEmail" =>
                    "Korisnik sa ovom email adresom već postoji.",

                "InvalidRoleName" =>
                    "Naziv uloge nije ispravan.",

                "DuplicateRoleName" =>
                    "Ova uloga već postoji.",

                "UserAlreadyHasPassword" =>
                    "Korisnik već ima postavljenu lozinku.",

                "UserLockoutNotEnabled" =>
                    "Zaključavanje korisničkog naloga nije omogućeno.",

                "UserAlreadyInRole" =>
                    "Korisnik već ima ovu ulogu.",

                "UserNotInRole" =>
                    "Korisnik nema ovu ulogu.",

                "PasswordTooShort" =>
                    "Lozinka mora imati najmanje 6 karaktera.",

                "PasswordRequiresNonAlphanumeric" =>
                    "Lozinka mora sadržavati barem jedan specijalni znak.",

                "PasswordRequiresDigit" =>
                    "Lozinka mora sadržavati barem jedan broj.",

                "PasswordRequiresLower" =>
                    "Lozinka mora sadržavati barem jedno malo slovo.",

                "PasswordRequiresUpper" =>
                    "Lozinka mora sadržavati barem jedno veliko slovo.",

                _ =>
                    "Došlo je do greške. Provjerite unesene podatke i pokušajte ponovo."
            };
        }
    }
}