using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using System.Text.Json;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class RezervacijaController : Controller
    {
        private readonly IRezervacijaService _service;
        private readonly UserManager<Korisnik> _userManager;

        public RezervacijaController(
            IRezervacijaService service,
            UserManager<Korisnik> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            if (!JeOsoblje())
                return RedirectToAction(nameof(Moje));

            var rezervacije = await _service.GetFiltriraneAsync(status);

            ViewBag.Status = status;
            ViewBag.Naslov = "Rezervacije";

            return View(rezervacije);
        }

        [HttpGet]
        public async Task<IActionResult> Moje(string? status)
        {
            if (JeOsoblje())
                return RedirectToAction(nameof(Index));

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return Unauthorized();

            var rezervacije = await _service
                .GetFiltriraneZaKorisnikaAsync(korisnik.Id, status);

            ViewBag.Status = status;
            ViewBag.Naslov = "Moje rezervacije";

            return View(rezervacije);
        }

        [HttpGet]
        public async Task<IActionResult> Historija(string? status)
        {
            if (JeOsoblje())
                return RedirectToAction(nameof(Index));

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return Unauthorized();

            var rezervacije = await _service
                .GetHistorijuZaKorisnikaAsync(korisnik.Id, status);

            ViewBag.Status = status;
            return View(rezervacije);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null)
                return NotFound();

            if (!await ImaPristupAsync(rezervacija))
                return Forbid();

            return View(rezervacija);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int parkingMjestoId)
        {
            if (JeOsoblje())
            {
                TempData["Error"] =
                    "Administrator i operater ne kreiraju lične rezervacije.";

                return RedirectToAction(nameof(Index));
            }

            if (TempData.ContainsKey("RezervacijaZaPlacanje"))
            {
                var json = TempData["RezervacijaZaPlacanje"]?.ToString();
                TempData.Keep("RezervacijaZaPlacanje");

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var sacuvana = JsonSerializer.Deserialize<Rezervacija>(json);

                    if (sacuvana != null &&
                        sacuvana.ParkingMjestoId == parkingMjestoId)
                    {
                        var priprema = await _service.PripremiKreiranjeAsync(
                            parkingMjestoId,
                            sacuvana.EmailZaObavijest);

                        if (priprema.ParkingMjesto == null)
                            return NotFound();

                        ViewBag.ParkingMjesto = priprema.ParkingMjesto;
                        return View(sacuvana);
                    }
                }
            }

            var result = await _service.PripremiKreiranjeAsync(
                parkingMjestoId,
                User.Identity?.Name ?? string.Empty);

            if (!result.Succeeded ||
                result.Rezervacija == null ||
                result.ParkingMjesto == null)
            {
                return NotFound();
            }

            ViewBag.ParkingMjesto = result.ParkingMjesto;
            return View(result.Rezervacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,ParkingMjestoId")]
            Rezervacija rezervacija)
        {
            if (JeOsoblje())
            {
                TempData["Error"] =
                    "Administrator i operater ne kreiraju lične rezervacije.";

                return RedirectToAction(nameof(Index));
            }

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return Unauthorized();

            var result = await _service
                .ObradiKreiranjeAsync(rezervacija, korisnik.Id);

            if (!ModelState.IsValid || !result.Succeeded)
            {
                AddErrorsToModelState(result.Errors);
                ViewBag.ParkingMjesto = result.ParkingMjesto;
                return View(rezervacija);
            }

            TempData["RezervacijaZaPlacanje"] =
                JsonSerializer.Serialize(result.Rezervacija);

            return RedirectToAction("Create", "Placanje");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null)
                return NotFound();

            if (!await ImaPristupAsync(rezervacija))
                return Forbid();

            return View(rezervacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,DatumKreiranja,VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,UkupnaCijena,StatusRezervacije,KorisnikId,ParkingMjestoId")]
            Rezervacija rezervacija)
        {
            if (id != rezervacija.Id)
                return NotFound();

            var postojeca = await _service.GetByIdAsync(id);
            if (postojeca == null)
                return NotFound();

            if (!await ImaPristupAsync(postojeca))
                return Forbid();

            if (!ModelState.IsValid)
                return View(rezervacija);

            var result = await _service.ObradiIzmjenuAsync(id, rezervacija);

            if (!result.Succeeded || result.Rezervacija == null)
            {
                AddErrorsToModelState(result.Errors);
                return View(rezervacija);
            }

            if (result.RequiresPayment)
            {
                TempData["IzmjenaRezervacijeZaPlacanje"] =
                    JsonSerializer.Serialize(result.Rezervacija);

                TempData["IznosDoplate"] =
                    result.IznosDoplate.ToString();

                return RedirectToAction("Create", "Placanje");
            }

            TempData["Success"] = "Rezervacija je uspješno ažurirana.";
            return RedirectNaListu();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null)
                return NotFound();

            if (!await ImaPristupAsync(rezervacija))
                return Forbid();

            return View(rezervacija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervacija = await _service.GetByIdAsync(id);
            if (rezervacija == null)
                return NotFound();

            if (!await ImaPristupAsync(rezervacija))
                return Forbid();

            var result = await _service.OtkaziAsync(id);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Error;
                return RedirectNaListu();
            }

            TempData["Success"] = result.Refunded
                ? "Rezervacija je otkazana, a plaćanje je refundirano."
                : "Rezervacija je uspješno otkazana.";

            return RedirectNaListu();
        }

        private bool JeOsoblje()
        {
            return User.IsInRole(Uloga.Administrator.ToString()) ||
                   User.IsInRole(Uloga.Operater.ToString());
        }

        private async Task<bool> ImaPristupAsync(Rezervacija rezervacija)
        {
            if (JeOsoblje())
                return true;

            var korisnik = await _userManager.GetUserAsync(User);
            return korisnik != null && rezervacija.KorisnikId == korisnik.Id;
        }

        private IActionResult RedirectNaListu()
        {
            return JeOsoblje()
                ? RedirectToAction(nameof(Index))
                : RedirectToAction(nameof(Moje));
        }

        private void AddErrorsToModelState(
            IReadOnlyDictionary<string, string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }
    }
}
