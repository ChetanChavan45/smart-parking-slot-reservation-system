using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaService _service;
        private readonly UserManager<Korisnik> _userManager;

        public RecenzijaController(
            IRecenzijaService service,
            UserManager<Korisnik> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var recenzije = await _service.GetAllAsync();
            return View(recenzije);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var recenzija = await _service.GetByIdAsync(id.Value);

            if (recenzija == null)
            {
                return NotFound();
            }

            return View(recenzija);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int rezervacijaId)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return Unauthorized();
            }

            var result = await _service.CheckEligibilityAsync(
                rezervacijaId,
                korisnik.Id);

            var failureResponse = HandleEligibilityFailure(result, rezervacijaId);
            if (failureResponse != null)
            {
                return failureResponse;
            }

            ViewBag.RezervacijaId = rezervacijaId;

            return View(new Recenzija
            {
                RezervacijaId = rezervacijaId,
                Ocjena = 5
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int rezervacijaId,
            Recenzija recenzija)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RezervacijaId = rezervacijaId;
                return View(recenzija);
            }

            var result = await _service.CreateForReservationAsync(
                rezervacijaId,
                korisnik.Id,
                recenzija);

            if (!result.Succeeded)
            {
                return HandleCreateFailure(result, rezervacijaId, recenzija);
            }

            TempData["Success"] = "Recenzija je uspješno dodana.";

            return RedirectToAction(
                "Details",
                "Rezervacija",
                new { id = rezervacijaId });
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var recenzija = await _service.GetByIdAsync(id.Value);

            if (recenzija == null)
            {
                return NotFound();
            }

            return View(recenzija);
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzija = await _service.GetByIdAsync(id);

            if (recenzija == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            TempData["Success"] = "Recenzija je obrisana.";

            return RedirectToAction(nameof(Index));
        }

        private IActionResult? HandleEligibilityFailure(
            RecenzijaEligibilityResult result,
            int rezervacijaId)
        {
            return result.Status switch
            {
                RecenzijaResultStatus.Success => null,
                RecenzijaResultStatus.NotFound => NotFound(),
                RecenzijaResultStatus.Forbidden => Forbid(),
                _ => RedirectToReservationWithError(
                    rezervacijaId,
                    result.ErrorMessage)
            };
        }

        private IActionResult HandleCreateFailure(
            RecenzijaCreateResult result,
            int rezervacijaId,
            Recenzija recenzija)
        {
            if (result.Status == RecenzijaResultStatus.NotFound)
            {
                return NotFound();
            }

            if (result.Status == RecenzijaResultStatus.Forbidden)
            {
                return Forbid();
            }

            ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage ?? "Recenziju nije moguće dodati.");

            ViewBag.RezervacijaId = rezervacijaId;
            return View(recenzija);
        }

        private IActionResult RedirectToReservationWithError(
            int rezervacijaId,
            string? errorMessage)
        {
            TempData["Error"] = errorMessage ?? "Recenziju nije moguće dodati.";

            return RedirectToAction(
                "Details",
                "Rezervacija",
                new { id = rezervacijaId });
        }
    }
}
