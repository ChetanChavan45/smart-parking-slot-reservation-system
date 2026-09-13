using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _service;

        public KorisnikController(IKorisnikService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Index() =>
            View(await _service.GetVisibleUsersAsync(User.IsInRole("Administrator")));

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();
            var korisnik = await _service.GetAccessibleByIdAsync(id, User.IsInRole("Administrator"));
            return korisnik == null ? NotFound() : View(korisnik);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Email,PhoneNumber,DatumRegistracije,StatusNaloga,Uloga")] Korisnik korisnik)
        {
            ValidateUser(korisnik);
            if (!ModelState.IsValid) return View(korisnik);

            await _service.CreateAsync(korisnik);
            TempData["Success"] = "Korisnik uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var korisnik = await _service.GetAccessibleByIdAsync(id, User.IsInRole("Administrator"));
            return korisnik == null ? NotFound() : View(korisnik);
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StatusNaloga statusNaloga, Uloga? uloga)
        {
            var result = await _service.UpdateAccountAsync(id, statusNaloga, uloga, User.IsInRole("Administrator"));
            if (result.NotFound) return NotFound();
            if (result.Forbidden) return Forbid();
            if (!result.Succeeded)
            {
                AddErrors(result.Errors);
                return View(result.Korisnik ?? await _service.GetByIdAsync(id));
            }

            TempData["Success"] = "Podaci naloga su uspješno ažurirani.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();
            var korisnik = await _service.GetByIdAsync(id);
            return korisnik == null ? NotFound() : View(korisnik);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Korisnik obrisan.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Profil()
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var korisnik = await _service.GetByIdAsync(userId);
            if (korisnik == null) return NotFound();

            ViewBag.BrojRezervacija = await _service.GetReservationCountAsync(userId);
            return View(korisnik);
        }

        [Authorize]
        public async Task<IActionResult> EditProfil()
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();
            var korisnik = await _service.GetByIdAsync(userId);
            return korisnik == null ? NotFound() : View(korisnik);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfil(Korisnik model, IFormFile? profilnaSlika)
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            ValidateUser(model);
            if (!ModelState.IsValid)
                return View(await _service.GetByIdAsync(userId));

            var result = await _service.UpdateProfileAsync(userId, model, profilnaSlika);
            if (result.NotFound) return NotFound();
            if (!result.Succeeded)
            {
                AddErrors(result.Errors, "ProfilnaSlikaUrl");
                return View(result.Korisnik ?? await _service.GetByIdAsync(userId));
            }

            TempData["Success"] = "Profil uspješno ažuriran.";
            return RedirectToAction(nameof(Profil));
        }

        [Authorize]
        public IActionResult PromijeniLozinku() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromijeniLozinku(string? trenutnaLozinka, string? novaLozinka, string? potvrdaLozinke)
        {
            if (string.IsNullOrWhiteSpace(trenutnaLozinka))
                ModelState.AddModelError("trenutnaLozinka", "Trenutna lozinka je obavezna.");
            if (string.IsNullOrWhiteSpace(novaLozinka))
                ModelState.AddModelError("novaLozinka", "Nova lozinka je obavezna.");
            if (string.IsNullOrWhiteSpace(potvrdaLozinke))
                ModelState.AddModelError("potvrdaLozinke", "Potvrda nove lozinke je obavezna.");
            if (!string.IsNullOrWhiteSpace(novaLozinka) && novaLozinka != potvrdaLozinke)
                ModelState.AddModelError("potvrdaLozinke", "Lozinke se ne podudaraju.");
            if (!ModelState.IsValid) return View();

            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _service.ChangePasswordAsync(userId, trenutnaLozinka!, novaLozinka!);
            if (result.NotFound) return NotFound();
            if (!result.Succeeded)
            {
                AddErrors(result.Errors);
                return View();
            }

            TempData["Success"] = "Lozinka uspješno promijenjena.";
            return RedirectToAction(nameof(Profil));
        }

        [Authorize(Roles = "Administrator,Operater")]
        public Task<IActionResult> Blokiraj(string id) => ChangeStatus(id, StatusNaloga.Blokiran, "Korisnik je blokiran.");

        [Authorize(Roles = "Administrator,Operater")]
        public Task<IActionResult> Aktiviraj(string id) => ChangeStatus(id, StatusNaloga.Aktivan, "Korisnik je aktiviran.");

        private async Task<IActionResult> ChangeStatus(string id, StatusNaloga status, string message)
        {
            var result = await _service.SetStatusAsync(id, status, User.IsInRole("Administrator"));
            if (result.NotFound) return NotFound();
            if (result.Forbidden) return Forbid();

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private void ValidateUser(Korisnik korisnik)
        {
            if (string.IsNullOrWhiteSpace(korisnik.Ime)) ModelState.AddModelError("Ime", "Ime je obavezno.");
            if (string.IsNullOrWhiteSpace(korisnik.Prezime)) ModelState.AddModelError("Prezime", "Prezime je obavezno.");
            if (string.IsNullOrWhiteSpace(korisnik.Email)) ModelState.AddModelError("Email", "Email adresa je obavezna.");
        }

        private void AddErrors(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors) ModelState.AddModelError(key, error);
        }
    }
}
