using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class HomeService : IHomeService
    {
        private readonly IRecenzijaService _recenzijaService;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IParkingMjestoService _parkingMjestoService;
        private readonly IKorisnikService _korisnikService;
        private readonly IPlacanjeService _placanjeService;

        public HomeService(
            IRecenzijaService recenzijaService,
            IRezervacijaService rezervacijaService,
            IParkingMjestoService parkingMjestoService,
            IKorisnikService korisnikService,
            IPlacanjeService placanjeService)
        {
            _recenzijaService = recenzijaService;
            _rezervacijaService = rezervacijaService;
            _parkingMjestoService = parkingMjestoService;
            _korisnikService = korisnikService;
            _placanjeService = placanjeService;
        }

        public async Task<HomeDashboardData> GetDashboardDataAsync(
            Korisnik? prijavljeniKorisnik)
        {
            var recenzijeTask = _recenzijaService.GetAllAsync();
            var rezervacijeTask = _rezervacijaService.GetAllAsync();
            var parkingMjestaTask = _parkingMjestoService.GetAllAsync();
            var korisniciTask = _korisnikService.GetAllAsync();
            var placanjaTask = _placanjeService.GetAllAsync();

            await Task.WhenAll(
                recenzijeTask,
                rezervacijeTask,
                parkingMjestaTask,
                korisniciTask,
                placanjaTask);

            var recenzije = await recenzijeTask;
            var rezervacije = await rezervacijeTask;
            var parkingMjesta = await parkingMjestaTask;
            var korisnici = await korisniciTask;
            var placanja = await placanjaTask;

            var mojeRezervacije = prijavljeniKorisnik == null
                ? new List<Rezervacija>()
                : await _rezervacijaService
                    .GetByKorisnikIdAsync(prijavljeniKorisnik.Id);

            return new HomeDashboardData
            {
                ProsjecnaOcjena = recenzije.Count == 0
                    ? 0
                    : recenzije.Average(r => r.Ocjena),
                BrojRecenzija = recenzije.Count,
                ZadnjeRecenzije = recenzije
                    .OrderByDescending(r => r.Datum)
                    .Take(3)
                    .ToList(),

                BrojRezervacija = rezervacije.Count,
                AktivneRezervacije = rezervacije.Count(r =>
                    r.StatusRezervacije == StatusRezervacije.Aktivna),
                ZavrseneRezervacije = rezervacije.Count(r =>
                    r.StatusRezervacije == StatusRezervacije.Zavrsena),
                OtkazaneRezervacije = rezervacije.Count(r =>
                    r.StatusRezervacije == StatusRezervacije.Otkazana),
                ZadnjeRezervacije = rezervacije
                    .OrderByDescending(r => r.DatumKreiranja)
                    .Take(5)
                    .ToList(),

                BrojParkingMjesta = parkingMjesta.Count,
                MjestaVanFunkcije = parkingMjesta
                    .Where(p => p.Status == StatusMjesta.VanFunkcije)
                    .Take(5)
                    .ToList(),

                BrojKorisnika = korisnici.Count,
                NoviKorisnici = korisnici
                    .OrderByDescending(k => k.DatumRegistracije)
                    .Take(5)
                    .ToList(),

                UkupnaZarada = (decimal)placanja
    .Where(p => p.StatusPlacanja == StatusPlacanja.Uspjesno)
    .Sum(p => p.Iznos),
                ZadnjaPlacanja = placanja
                    .OrderByDescending(p => p.DatumPlacanja)
                    .Take(5)
                    .ToList(),

                MojeRezervacijeCount = mojeRezervacije.Count,
                MojaAktivnaRezervacija = mojeRezervacije
                    .Where(r =>
                        r.StatusRezervacije == StatusRezervacije.Aktivna)
                    .OrderBy(r => r.VrijemePocetka)
                    .FirstOrDefault()
            };
        }
    }
}
