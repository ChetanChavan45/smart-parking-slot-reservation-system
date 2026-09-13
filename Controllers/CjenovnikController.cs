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
    public class CjenovnikController : Controller
    {
        private readonly ICjenovnikService _service;

        public CjenovnikController(ICjenovnikService service)
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

            var cjenovnik = await _service.GetByIdAsync(id.Value);

            if (cjenovnik == null) return NotFound();

            return View(cjenovnik);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CijenaPoSatu,VaziOd,VaziDo")] Cjenovnik cjenovnik)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(cjenovnik);
                return RedirectToAction(nameof(Index));
            }

            return View(cjenovnik);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cjenovnik = await _service.GetByIdAsync(id.Value);

            if (cjenovnik == null) return NotFound();

            return View(cjenovnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CijenaPoSatu,VaziOd,VaziDo")] Cjenovnik cjenovnik)
        {
            if (id != cjenovnik.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(cjenovnik);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CjenovnikExists(cjenovnik.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(cjenovnik);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cjenovnik = await _service.GetByIdAsync(id.Value);

            if (cjenovnik == null) return NotFound();

            return View(cjenovnik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CjenovnikExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}