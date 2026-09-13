using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class PodrskaController : Controller
    {
        private readonly IPodrskaService _service;

        public PodrskaController(IPodrskaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var mozePregledatiSve =
                User.IsInRole("Administrator") || User.IsInRole("Operater");

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var zahtjevi = await _service.GetZahtjeveAsync(
                korisnikId,
                mozePregledatiSve,
                status);

            ViewBag.Status = status;
            return View(zahtjevi);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PodrskaZahtjev
            {
                Prioritet = PrioritetPodrske.Srednji,
                Kategorija = KategorijaPodrske.Ostalo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naslov,Opis,Kategorija,Prioritet")] PodrskaZahtjev zahtjev)
        {
            if (!ModelState.IsValid)
                return View(zahtjev);

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(korisnikId))
                return Unauthorized();

            await _service.KreirajAsync(zahtjev, korisnikId);
            TempData["Success"] = "Zahtjev za podršku je uspješno poslan.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var zahtjev = await _service.GetByIdAsync(id.Value);

            if (zahtjev == null)
                return NotFound();

            if (!MozePristupiti(zahtjev))
                return Forbid();

            return View(zahtjev);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Odgovori(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var zahtjev = await _service.GetByIdAsync(id.Value);
            return zahtjev == null ? NotFound() : View(zahtjev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Odgovori(
            int id,
            string odgovor,
            StatusPodrske status)
        {
            if (string.IsNullOrWhiteSpace(odgovor))
            {
                TempData["Error"] = "Odgovor ne može biti prazan.";
                return RedirectToAction(nameof(Odgovori), new { id });
            }

            var uspjesno = await _service.OdgovoriAsync(id, odgovor, status);

            if (!uspjesno)
                return NotFound();

            TempData["Success"] =
                $"Zahtjev je ažuriran. Novi status: {_service.FormatirajStatus(status)}.";

            return RedirectToAction(nameof(Index), new { status = status.ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var zahtjev = await _service.GetByIdAsync(id);

            if (zahtjev == null)
                return NotFound();

            await _service.DeleteAsync(id);
            TempData["Success"] = "Zahtjev za podršku je uspješno obrisan.";

            return RedirectToAction(nameof(Index));
        }

        private bool MozePristupiti(PodrskaZahtjev zahtjev)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return true;

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrWhiteSpace(korisnikId) &&
                   zahtjev.KorisnikId == korisnikId;
        }
    }
}
