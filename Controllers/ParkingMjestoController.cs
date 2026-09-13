using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class ParkingMjestoController : Controller
    {
        private readonly IParkingMjestoService _service;

        public ParkingMjestoController(IParkingMjestoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost)
        {
            var parkingMjesta = await _service.SearchAsync(
                zonaId,
                tipMjesta,
                natkriveno,
                minCijena,
                maxCijena,
                maxUdaljenost);

            var korisnikJePretrazivao =
                zonaId.HasValue ||
                tipMjesta.HasValue ||
                natkriveno.HasValue ||
                minCijena.HasValue ||
                maxCijena.HasValue ||
                maxUdaljenost.HasValue;

            if (korisnikJePretrazivao)
            {
                var preporucenoMjesto =
                    await _service.GetRecommendedAsync(parkingMjesta);

                ViewBag.PreporucenoMjestoId = preporucenoMjesto?.Id;
            }
            else
            {
                ViewBag.PreporucenoMjestoId = null;
            }

            return View(parkingMjesta);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);

            if (parkingMjesto == null)
                return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")]
            ParkingMjesto parkingMjesto)
        {
            if (!ModelState.IsValid)
                return View(parkingMjesto);

            await _service.AddAsync(parkingMjesto);

            TempData["Success"] = "Parking mjesto je uspješno dodano.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);

            if (parkingMjesto == null)
                return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")]
            ParkingMjesto parkingMjesto)
        {
            if (id != parkingMjesto.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(parkingMjesto);

            try
            {
                await _service.UpdateAsync(parkingMjesto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _service.ExistsAsync(parkingMjesto.Id))
                    return NotFound();

                throw;
            }

            TempData["Success"] = "Parking mjesto je uspješno ažurirano.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);

            if (parkingMjesto == null)
                return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = "Parking mjesto je uspješno obrisano.";
            return RedirectToAction(nameof(Index));
        }
    }
}
