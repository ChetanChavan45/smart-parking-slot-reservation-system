using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(
            IHomeService homeService,
            UserManager<Korisnik> userManager)
        {
            _homeService = homeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            Korisnik? prijavljeniKorisnik = null;

            if (User.Identity?.IsAuthenticated == true &&
                !User.IsInRole("Administrator") &&
                !User.IsInRole("Operater"))
            {
                prijavljeniKorisnik = await _userManager.GetUserAsync(User);
            }

            var data = await _homeService
                .GetDashboardDataAsync(prijavljeniKorisnik);

            ViewBag.ProsjecnaOcjena = data.ProsjecnaOcjena;
            ViewBag.BrojRecenzija = data.BrojRecenzija;
            ViewBag.ZadnjeRecenzije = data.ZadnjeRecenzije;

            ViewBag.BrojRezervacija = data.BrojRezervacija;
            ViewBag.AktivneRezervacije = data.AktivneRezervacije;
            ViewBag.ZavrseneRezervacije = data.ZavrseneRezervacije;
            ViewBag.OtkazaneRezervacije = data.OtkazaneRezervacije;
            ViewBag.ZadnjeRezervacije = data.ZadnjeRezervacije;

            ViewBag.BrojParkingMjesta = data.BrojParkingMjesta;
            ViewBag.MjestaVanFunkcije = data.MjestaVanFunkcije;

            ViewBag.BrojKorisnika = data.BrojKorisnika;
            ViewBag.NoviKorisnici = data.NoviKorisnici;

            ViewBag.UkupnaZarada = data.UkupnaZarada;
            ViewBag.ZadnjaPlacanja = data.ZadnjaPlacanja;

            ViewBag.MojeRezervacijeCount = data.MojeRezervacijeCount;
            ViewBag.MojaAktivnaRezervacija = data.MojaAktivnaRezervacija;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
