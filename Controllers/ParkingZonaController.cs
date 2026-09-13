using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PametniParkingSistem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ParkingZonaController : Controller
    {
        private readonly IParkingZonaService _service;

        public ParkingZonaController(IParkingZonaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var parkingZona = await _service.GetByIdAsync(id.Value);

            if (parkingZona == null) return NotFound();

            return View(parkingZona);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Lokacija,Opis,ProsjecnaOcjena")] ParkingZona parkingZona)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(parkingZona);
                return RedirectToAction(nameof(Index));
            }

            return View(parkingZona);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var parkingZona = await _service.GetByIdAsync(id.Value);

            if (parkingZona == null) return NotFound();

            return View(parkingZona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Lokacija,Opis,ProsjecnaOcjena")] ParkingZona parkingZona)
        {
            if (id != parkingZona.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(parkingZona);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ParkingZonaExists(parkingZona.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(parkingZona);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var parkingZona = await _service.GetByIdAsync(id.Value);

            if (parkingZona == null) return NotFound();

            return View(parkingZona);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ParkingZonaExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}