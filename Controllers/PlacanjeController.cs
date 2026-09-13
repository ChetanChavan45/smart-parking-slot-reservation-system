using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.ViewModels.Placanje;
using System.Text.Json;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class PlacanjeController : Controller
    {
        private readonly IPlacanjeService _placanjeService;

        public PlacanjeController(IPlacanjeService placanjeService)
        {
            _placanjeService = placanjeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _placanjeService.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var context = await UcitajPlacanjeContextAsync();

            if (context == null)
                return RedirectToOdgovarajucuStranicu();

            PrimijeniContextNaView(context, context.Model);
            return View(context.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlacanjeRezervacijeViewModel model)
        {
            var context = await UcitajPlacanjeContextAsync();

            if (context == null)
                return RedirectToOdgovarajucuStranicu();

            model.Iznos = context.Model.Iznos;

            if (!ModelState.IsValid)
            {
                PrimijeniContextNaView(context, model);
                return View(model);
            }

            PlacanjeProcessResult result;

            if (context.TipPlacanja == TipPlacanja.NovaRezervacija)
            {
                result = await _placanjeService.ObradiNovuRezervacijuAsync(
                    context.Rezervacija,
                    model);
            }
            else
            {
                result = await _placanjeService.ObradiDoplatuAsync(
                    context.Rezervacija,
                    context.IznosDoplate,
                    model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                PrimijeniContextNaView(context, model);
                return View(model);
            }

            OcistiTempData(context.TipPlacanja);

            TempData["Success"] = context.TipPlacanja == TipPlacanja.NovaRezervacija
                ? "Rezervacija je uspješno plaćena i potvrđena."
                : "Doplata je uspješno izvršena i rezervacija je ažurirana.";

            return RedirectToAction("Index", "Rezervacija");
        }

        private async Task<PlacanjeContext?> UcitajPlacanjeContextAsync()
        {
            if (TempData.ContainsKey("RezervacijaZaPlacanje"))
            {
                var rezervacija = ProcitajRezervacijuIzTempData("RezervacijaZaPlacanje");

                if (rezervacija == null)
                    return null;

                TempData.Keep("RezervacijaZaPlacanje");

                var parkingMjestoNaziv = await _placanjeService
                    .GetParkingMjestoNazivAsync(rezervacija.ParkingMjestoId);

                return new PlacanjeContext
                {
                    TipPlacanja = TipPlacanja.NovaRezervacija,
                    Rezervacija = rezervacija,
                    ParkingMjestoNaziv = parkingMjestoNaziv,
                    Model = new PlacanjeRezervacijeViewModel
                    {
                        Iznos = rezervacija.UkupnaCijena
                    }
                };
            }

            if (TempData.ContainsKey("IzmjenaRezervacijeZaPlacanje"))
            {
                var rezervacija = ProcitajRezervacijuIzTempData(
                    "IzmjenaRezervacijeZaPlacanje");

                if (rezervacija == null ||
                    !TempData.TryGetValue("IznosDoplate", out var iznosValue) ||
                    !double.TryParse(iznosValue?.ToString(), out var iznosDoplate))
                {
                    return null;
                }

                TempData.Keep("IzmjenaRezervacijeZaPlacanje");
                TempData.Keep("IznosDoplate");

                return new PlacanjeContext
                {
                    TipPlacanja = TipPlacanja.Doplata,
                    Rezervacija = rezervacija,
                    IznosDoplate = iznosDoplate,
                    Model = new PlacanjeRezervacijeViewModel
                    {
                        Iznos = iznosDoplate
                    }
                };
            }

            return null;
        }

        private Rezervacija? ProcitajRezervacijuIzTempData(string key)
        {
            var json = TempData[key]?.ToString();

            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Rezervacija>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private void PrimijeniContextNaView(
            PlacanjeContext context,
            PlacanjeRezervacijeViewModel model)
        {
            model.Iznos = context.Model.Iznos;
            ViewBag.Rezervacija = context.Rezervacija;
            ViewBag.TipPlacanja = context.TipPlacanja == TipPlacanja.NovaRezervacija
                ? "NovaRezervacija"
                : "Doplata";

            if (!string.IsNullOrWhiteSpace(context.ParkingMjestoNaziv))
                ViewBag.ParkingMjestoNaziv = context.ParkingMjestoNaziv;
        }

        private IActionResult RedirectToOdgovarajucuStranicu()
        {
            return TempData.ContainsKey("IzmjenaRezervacijeZaPlacanje")
                ? RedirectToAction("Index", "Rezervacija")
                : RedirectToAction("Index", "ParkingMjesto");
        }

        private void OcistiTempData(TipPlacanja tipPlacanja)
        {
            if (tipPlacanja == TipPlacanja.NovaRezervacija)
            {
                TempData.Remove("RezervacijaZaPlacanje");
                return;
            }

            TempData.Remove("IzmjenaRezervacijeZaPlacanje");
            TempData.Remove("IznosDoplate");
        }

        private enum TipPlacanja
        {
            NovaRezervacija,
            Doplata
        }

        private sealed class PlacanjeContext
        {
            public required TipPlacanja TipPlacanja { get; init; }
            public required Rezervacija Rezervacija { get; init; }
            public required PlacanjeRezervacijeViewModel Model { get; init; }
            public double IznosDoplate { get; init; }
            public string? ParkingMjestoNaziv { get; init; }
        }
    }
}
