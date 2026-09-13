using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class EmailObavijestController : Controller
    {
        private readonly IEmailObavijestService _service;

        public EmailObavijestController(IEmailObavijestService service)
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

            var emailObavijest = await _service.GetByIdAsync(id.Value);

            if (emailObavijest == null) return NotFound();

            return View(emailObavijest);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Primalac,Naslov,Sadrzaj,DatumSlanja,TipObavijesti,StatusEmaila")] EmailObavijest emailObavijest)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(emailObavijest);
                return RedirectToAction(nameof(Index));
            }

            return View(emailObavijest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var emailObavijest = await _service.GetByIdAsync(id.Value);

            if (emailObavijest == null) return NotFound();

            return View(emailObavijest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Primalac,Naslov,Sadrzaj,DatumSlanja,TipObavijesti,StatusEmaila")] EmailObavijest emailObavijest)
        {
            if (id != emailObavijest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(emailObavijest);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmailObavijestExists(emailObavijest.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(emailObavijest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var emailObavijest = await _service.GetByIdAsync(id.Value);

            if (emailObavijest == null) return NotFound();

            return View(emailObavijest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmailObavijestExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}